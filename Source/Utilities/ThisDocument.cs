/*
 * User: Donovan Justice
 * Date: 6/1/2018
 * Time: 5:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Utilities
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.DB.Macros.AddInId("22BA5A66-BE30-439D-8721-6F17B59DEDEE")]
	public partial class ThisDocument
	{
		private void Module_Startup(object sender, EventArgs e)
		{
			TaskDialog dialog = new TaskDialog("Bulk Schedule CSV Parser");
            dialog.MainInstruction = "Run Schedule Parser?";
            dialog.MainContent = "Would you like to start the Bulk Schedule CSV Parser macro now?";
            dialog.CommonButtons = TaskDialogCommonButtons.No | TaskDialogCommonButtons.Yes;
            dialog.DefaultButton = TaskDialogResult.No;
            TaskDialogResult tResult = dialog.Show();
            
            if (tResult == TaskDialogResult.Yes)
            {
            	ScheduleCSVParser();
            }
		}

		private void Module_Shutdown(object sender, EventArgs e)
		{

		}
		
		public static string DestinationFilepath { get; set; }
		
		public void ScheduleCSVParser()
		{
			ProgressBarHandler.Instance.Clear();
			
			string sourceDirectory = GetPath("Select the Project Base Directory.");
			
			if (string.IsNullOrEmpty(sourceDirectory))
			{
				if (sourceDirectory == null)
				{
					TaskDialog.Show("Bulk Schedule CSV Parser", "Bulk Schedule CSV Parser has been cancelled.");
					return;
				}
				
				if (sourceDirectory == string.Empty)
				{
					TaskDialog.Show("Error", "The specified directory is invalid. Please try again!");
					return;
				}
			}
			
			List<string> filePaths = GetFilePaths(sourceDirectory);
			
			if (filePaths.Count == 0)
			{
				TaskDialog.Show("Error", "The specified source directory does not contain any Revit project files (.rvt). Please try again.");
				return;
			}
			else
			{
				TaskDialog.Show("Bulk Schedule CSV Parser", string.Format("The source directory {0} has {1} project file(s)", sourceDirectory, filePaths.Count));
				
				string destPath = GetPath("Select Destination for .csv file", sourceDirectory);
				
				if (string.IsNullOrEmpty(destPath))
				{
					if (destPath == null)
					{
						TaskDialog.Show("Bulk Schedule CSV Parser", "Bulk Schedule CSV Parser has been cancelled.");
						return;
					}
					
					if (destPath == string.Empty)
					{
						TaskDialog.Show("Error", "The specified destination directory is invalid. Please try again.");
						return;
					}
				}
				
				DestinationFilepath = Path.Combine(destPath, string.Format("schedules_parser_{0}.csv", DateTime.Now.ToString("yyyy-dd-MM_HH-mm-ss")));
				
				ProgressBarHandler.Instance.Show(string.Format("Bulk Schedule CSV Parser: {0}", DestinationFilepath));
				
				InitializeCsv();
				var exportDirectory = InitializeExportDirectory();
				
				RunScheduleParser(filePaths);

                ProgressBarHandler.Instance.Close();
                
                if (!ProgressBarHandler.Instance.IsCanceled)
					PromptOpenLocationDialog();
			}
		}
				
		public void RunScheduleParser(List<string> filePaths)
		{
            ProgressBarHandler.Instance.NextCommand(filePaths.Count, "Parsing");

            foreach (string filePath in filePaths)
            {
                this.Application.Application.FailuresProcessing += Application_FailuresProcessing;

                if (ProgressBarHandler.Instance.IsCanceled)
                {
                    this.Application.Application.FailuresProcessing -= Application_FailuresProcessing;
                    break;
                }

                bool canOpenDocument = false;

                Document document = null;

                string fileName = Path.GetFileName(filePath);
                string fileHash = "UNKNOWN";

                var openDocument = OpenDocument(filePath);

                if (openDocument != null)
                {
                    document = openDocument;
                    canOpenDocument = true;
                }
                else
                {
                    try
                    {
                        document = this.Application.Application.OpenDocumentFile(filePath);
                        fileHash = filePath.ComputeHash();
                        canOpenDocument = true;
                    }
                    catch (Exception ex)
                    {
                        TaskDialog.Show("Error", string.Format("Unable to open document {0} - ERROR - {1}", fileName, ex.ToString()));
                    }
                }

                if (canOpenDocument)
                {
                    var schedules = new FilteredElementCollector(document)
                        .OfClass(typeof(ViewSchedule)).OfType<ViewSchedule>()
                        .Where(x => !x.IsTitleblockRevisionSchedule && !x.IsTemplate && x.Definition.ShowHeaders)
                        .OrderBy(x => x.Name)
                        .ToList();

                    ProgressBarHandler.Instance.ProgressMainProcess(schedules.Count, string.Format("{0} Schedules from {1}", schedules.Count, fileName));

                    foreach (ViewSchedule schedule in schedules)
                    {
                        if (ProgressBarHandler.Instance.IsCanceled)
                            break;

                        try
                        {
                            ParseSchedule(schedule, filePath, fileHash);
                            ProgressBarHandler.Instance.ProgressSubProcess(string.Format("Processing: {0}", schedule.Name));
                        }
                        catch (Exception exception)
                        {
                            TaskDialog.Show(string.Format("An exception occurred for schedule {0} - ERROR", schedule.Name), exception.ToString());
                        }
                    }

                    if (openDocument == null)
                        document.Close(false);
                }

                this.Application.Application.FailuresProcessing -= Application_FailuresProcessing;
            }

            ProgressBarHandler.Instance.IsComplete = true;
		}
			
		public void ParseSchedule(ViewSchedule schedule, string filePath, string fileHash)
		{
			if (!schedule.IsTitleblockRevisionSchedule && !schedule.IsTemplate && schedule.Definition.ShowHeaders)
			{
				var scheduleFieldIds = schedule.Definition.GetFieldOrder();
				List<ScheduleLine> lines = new List<ScheduleLine>();
				
				var indexCount = 0;
				for (int i = 0; i < scheduleFieldIds.Count; i++)
				{
					var scheduleField = schedule.Definition.GetField(scheduleFieldIds[i]);
					
					if (scheduleField.IsHidden)
						continue;
					
					var idString = string.Empty;
					var isShared = false;
					
					if (scheduleField.ParameterId.IntegerValue > 0)
					{
						var elem = schedule.Document.GetElement(scheduleField.ParameterId);
						
						if (elem is SharedParameterElement)
						{
							SharedParameterElement shElem = elem as SharedParameterElement;
							idString = shElem.GuidValue.ToString();
							isShared = true;
						}
					}
					
					var scheduleLine = new ScheduleLine()
					{
						Filepath = filePath,
						Filename = Path.GetFileName(filePath),
						Hash = fileHash,
						ScheduleTitle = schedule.Name,
						OriginalColumnHeader = scheduleField.ColumnHeading.Trim(),
						ParameterName = scheduleField.GetName().Trim(),
						SharedParameterGuid = idString,
						FieldType = scheduleField.FieldType,
						IsShared = isShared,
						ColumnIndex = indexCount
					};
					
					lines.Add(scheduleLine);
					indexCount++;					
				}
				
				var sectionData = schedule.GetTableData().GetSectionData(SectionType.Body);
					
				for (int col = sectionData.FirstColumnNumber; col <= sectionData.LastColumnNumber; col++)
				{
					var columnHeaders = string.Empty;
					var columnValues = string.Empty;
					
					var matchingLine = lines.FirstOrDefault(x => x.ColumnIndex == col);

					if (matchingLine != null)
					{
						bool isMatchingOriginalText = false;
						TableMergedCell previousMergedData = null;
						
						var previousMergedCellTopRowNumber = sectionData.LastRowNumber;
						var previousMergedCellLeftRowNumber = sectionData.FirstColumnNumber;
						
						//Read each cell of the column in reverse order to build up values and headers
						for (int row = sectionData.LastRowNumber; row >= sectionData.FirstRowNumber; row--)
						{
							var mergedData = sectionData.GetMergedCell(row, col);
							
							var mergedTopLeftCellText = sectionData.GetCellText(mergedData.Top, mergedData.Left).Trim();
							var mergedBottomLeftCellText = sectionData.GetCellText(mergedData.Bottom, mergedData.Left).Trim();

							bool isMergedAcrossRowsInColumn = false;
							
							if (row != sectionData.LastRowNumber)
								isMergedAcrossRowsInColumn = IsMergedInColumnOnly(mergedData, previousMergedData);
							
							if (!isMatchingOriginalText)
								isMatchingOriginalText = (matchingLine.OriginalColumnHeader == mergedTopLeftCellText);

							if (isMatchingOriginalText)
							{
								if (isMergedAcrossRowsInColumn)
								{
									//The merged column value has already been found from the previous cell's merged data; skip to next row
									row = mergedData.Top;
								}
								else
								{
									columnHeaders = (!string.IsNullOrEmpty(columnHeaders)) 
										? string.Format("{0}|{1}", mergedTopLeftCellText.Escape(), columnHeaders) 
										: mergedTopLeftCellText.Escape();
								}
							}
							else
							{
								columnValues = (!string.IsNullOrEmpty(columnValues)) 
									? string.Format("{0}|{1}", mergedTopLeftCellText.Escape(), columnValues) 
									: mergedTopLeftCellText.Escape();
							}
							
							previousMergedCellTopRowNumber = mergedData.Top;
							previousMergedCellLeftRowNumber = mergedData.Left;
							
							previousMergedData = mergedData;
						}
						
						matchingLine.DelimitedColumnHeaders = columnHeaders;
						matchingLine.ColumnValues = columnValues;
					}
				}

				WriteCsv(lines);
				
				lines.Clear();
			}
		}
		
		#region Revit Macros generated code
		
		private void InternalStartup()
		{
			this.Startup += new System.EventHandler(Module_Startup);
			this.Shutdown += new System.EventHandler(Module_Shutdown);
		}
		
		#endregion Revit Macros generated code
		
		#region Private

		private void PromptOpenLocationDialog()
		{
			TaskDialog td = new TaskDialog("Bulk Schedule CSV Parser")
            {
            	MainInstruction = "Show .csv file in File Explorer?",
            	CommonButtons = TaskDialogCommonButtons.No | TaskDialogCommonButtons.Yes,
            	DefaultButton = TaskDialogResult.Yes
            };

            if (td.Show() == TaskDialogResult.Yes)
                OpenLocation(DestinationFilepath);
		}
		
		private Document OpenDocument(string pathName)
        {
            Document matchedDocument = null;
            Document possibleMatchedDocument = null;

            foreach (Document openDocument in this.Application.Application.Documents)
            {
                if (openDocument.PathName == pathName)
                {
                    matchedDocument = openDocument;
                }
                else if (openDocument.PathName.Contains(Path.GetFileName(pathName)))
                {
                    possibleMatchedDocument = openDocument;
                }
            }

            return (matchedDocument != null) ? matchedDocument : possibleMatchedDocument;
        }	
		
		private static bool IsMergedInColumnOnly(TableMergedCell mergedData, TableMergedCell previousMergedData)
		{
			var ret = ( mergedData.Left == previousMergedData.Left
				     && mergedData.Top == previousMergedData.Top );
			
			return ret;
		}
		
		private static string ExportScheduleCSV(ViewSchedule schedule, string scheduleExportDirectory)
		{
			ViewScheduleExportOptions exportOptions = new ViewScheduleExportOptions()
			{
				ColumnHeaders = ExportColumnHeaders.MultipleRows,
				Title = false
			};
			
			var scheduleExportPath = Path.Combine(scheduleExportDirectory, string.Format("{0}.csv", RemoveInvalidCharacters(schedule.Name)));
			schedule.Export(scheduleExportDirectory, Path.GetFileName(scheduleExportPath), exportOptions);
			
			return scheduleExportPath;
		}
		
		private static string RemoveInvalidCharacters(string name)
		{
			string str = name;
			
			foreach (char ch in new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()))
			{
				str = str.Replace(ch.ToString(), "");
			}
			
			return str;
		}
		
		private static string InitializeExportDirectory()
		{
			var scheduleExportDirectory = Path.Combine(Path.GetTempPath(), "ScheduleExporter");
				
			if (!Directory.Exists(scheduleExportDirectory))
				Directory.CreateDirectory(scheduleExportDirectory);
			
			return scheduleExportDirectory;
		}
		
		private static void InitializeCsv()
		{
			using (FileStream stream = new FileStream(DestinationFilepath, FileMode.Append, FileAccess.Write))
			using (StreamWriter writer = new StreamWriter(stream))
			{
				writer.WriteLine(@"""Filename"",""File Hash ID"",""Filepath"",""Schedule Title"",""Column Headers"",""Parameter Name"",""Is Shared"",""Shared Parameter GUID"",""Field Type"",""Column Values""");
			}
		}
		
		private static void WriteCsv(List<ScheduleLine> lines)
		{
			using (FileStream stream = new FileStream(DestinationFilepath, FileMode.Append, FileAccess.Write))
			using (StreamWriter writer = new StreamWriter(stream))
			{
				foreach (ScheduleLine line in lines)
				{
					object[] args = new object[] { line.Filename.Escape(), line.Hash.Escape(), line.Filepath.Escape(), line.ScheduleTitle.Escape(), line.DelimitedColumnHeaders.Escape(), line.ParameterName.Escape(), line.IsShared.ToString().Escape(), line.SharedParameterGuid.Escape(), line.FieldType.ToString(), line.ColumnValues.Escape() };
					writer.WriteLine("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\"", args);
				}
			}
		}
	
		private static string GetPath(string title, string startDirectory = "")
		{
			string selectedPath = string.Empty;
			
			FolderBrowserDialog dialog = new FolderBrowserDialog 
			{
				Description = title,
				SelectedPath = startDirectory
			};
			
			DialogResult result = dialog.ShowDialog();
			
			if (result == DialogResult.OK)
			{
				selectedPath = dialog.SelectedPath;
			}
			else if (result == DialogResult.Cancel)
			{
				selectedPath = null;
			}
			
			return selectedPath;
		}
		
		private static List<string> GetFilePaths(string selectedPath)
		{
			List<string> list = new List<string>(Directory.GetFiles(selectedPath, "*.rvt", SearchOption.AllDirectories));
			
			foreach (string str in list)
			{
				FileInfo info = new FileInfo(str) 
				{
					IsReadOnly = false
				};
			}
			
			return list;
		}
		
		private void OpenLocation(string location)
        {
            string args = string.Format("/select,\"{0}\"", location);

            System.Diagnostics.Process.Start("explorer.exe", args);
        }
		
		private void Application_FailuresProcessing(object sender, FailuresProcessingEventArgs e)
        {
            FailuresAccessor failuresAccessor = e.GetFailuresAccessor();

            failuresAccessor.DeleteAllWarnings();

            IList<FailureMessageAccessor> fmas = failuresAccessor.GetFailureMessages();

            var options = failuresAccessor.GetFailureHandlingOptions();
            options.SetClearAfterRollback(true);
            options.SetForcedModalHandling(false);

            failuresAccessor.SetFailureHandlingOptions(options);
        }
				
		#endregion Private
	}
	
	public static class Extensions
	{
		public static string Escape(this string value)
		{
			return (string.IsNullOrEmpty(value)) 
				? string.Empty 
				: value.Replace("\"", "\"\"");
		}
		
		public static string ComputeHash(this string filepath)
		{
			string str = string.Empty;
			FileInfo info = new FileInfo(filepath);
			
			try
			{
				str = Convert.ToBase64String(SHA256.Create().ComputeHash(info.OpenRead()));
			}
			catch (Exception)
			{ 
				str = "Unknown";
			}
			
			return str;
		}
	}	
		
	public class ScheduleLine
	{
		public string Hash { get; set; }
		public string Filepath { get; set; }
		public string Filename { get; set; }
		public string ScheduleTitle { get; set; }
		public string OriginalColumnHeader { get; set; }
		public string DelimitedColumnHeaders { get; set; }
		public string ParameterName { get; set; }
		public string SharedParameterGuid { get; set; }
		public bool IsShared { get; set; }
		public ScheduleFieldType FieldType { get; set; }
		public int ColumnIndex { get; set; }
		public string ColumnValues { get; set; }
	}
}
