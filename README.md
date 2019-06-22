## About
The Revit Schedule CSV Parser is a Revit utility that performs a batch extraction to CSV of all Revit schedules from all Revit files in a folder and subfolders. It was developed by Donovan Justice and Randall Stevens of [AVAIL](https://www.getavail.com/About) as part of the Building Content Summit presentation "Identifying Data Patterns from Professional Practice" by Van Woods. With the permissive MIT open source license from AVAIL, derivatives, enhancments, fixes, documentation, and participation in the submission of crowd sourced data for industry-wide analysis is encouraged.

## Data Extracted
The following data points are extracted:
(code flag)[OnSheet], [File GUID], [Column Headers], [Column Values],  
1. [finish]
1. [finish]
1. [finish]
1. [finish]
1. [finish]

~~~~private static void InitializeCsv()
		{
			using (FileStream stream = new FileStream(DestinationFilepath, FileMode.Append, FileAccess.Write))
			using (StreamWriter writer = new StreamWriter(stream))
			{
				writer.WriteLine(@"""Filename"",""Schedule Title"",""Column Headers"",""Parameter Name"",""Is Shared"",""Shared Parameter Guid"",""Field Type"",""File Hash"",""Filepath"",""Column Values""");
			}
		}
		
		private static void WriteCsv(List<ScheduleLine> lines)
		{
			using (FileStream stream = new FileStream(DestinationFilepath, FileMode.Append, FileAccess.Write))
			using (StreamWriter writer = new StreamWriter(stream))
			{
				foreach (ScheduleLine line in lines)
				{
					object[] args = new object[] { line.Filename.Escape(), line.ScheduleTitle.Escape(), line.DelimitedColumnHeaders.Escape(), line.ParameterName.Escape(), line.IsShared.ToString().Escape(), line.SharedParameterGuid.Escape(), line.FieldType.ToString(), line.Hash.Escape(), line.Filepath.Escape(), line.ColumnValues.Escape() };
					writer.WriteLine("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\"", args);
				}
			}
		}
~~~~


## To Extract Revit Schedule Data
1. [finish]
1. [finish]
1. [finish]

## To Contribute Code
1. [finish]
1. [finish]
1. [finish]

## License
[finish]

## Disclaimer (MOVE TO PROFILE)
All views are my own and are not of my employers. Links do not imply endorsement of any kind.
