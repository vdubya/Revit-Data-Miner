## Quick Start
* Download and run [ScheduleCSVParser-v0.1.rvt](https://github.com/vdubya/Revit-Schedule-CSV-Parser/releases/download/v0.1/ScheduleCSVParser-v0.1.rvt)
* Select source folder with RVT files to scan (includes all subfolders) and destination folder for CSV export.
* Create a new Github Issue and attach the exported CSV to contribute to crowdsourced data analysis.

## About
* The Revit Schedule CSV Parser is a Revit utility that performs a batch extraction to CSV of all Revit schedules from all Revit files in a folder and subfolders. 
* Originally developed by Donovan Justice and Randall Stevens of [AVAIL](https://www.getavail.com/About) as part of the Building Content Summit presentation "Identifying Data Patterns from Professional Practice" by Van Woods. 
* Licensed with a permissive MIT open source license from AVAIL.
* Users are encouraged to develop derivatives, enhancements, fixes, documentation, and participate in the submission of crowdsourced data for industry-wide analysis. 
* Please submit improvements. To encourage maximum participation, growth, and innovation, users are requested (but not required) to submit Pull Requests to incorporate improvements to the source. 
* CSV was chosen as a universally accessible format, and commonly used in many open source data science and machine learning tools. 
* A web accessible database is under consideration for collecting a curated set of bulk schedule extraction contributions for statistical analyses. Please contact Van Woods if you are interested in participating. See Contribution Guidelines [FINISH]. 

## Data Extracted
CSV with quote wrapped strings require the least amount of steps for opening in Excel. 
The following data is extracted in the following comma delimited format with quote wrapped strings: 
~~~~csharp
"ID","ScheduleTitle","InOnSheet","ColumnHeaders","ParameterName","IsShared","SharedParameterGUID","Column Values","FileHashID","Filename","Filepath"
~~~~

1. **ID**: Sequential unique ID based on the order of schedule columns 
1. **ScheduleTitle**: Title of drawing schedule if displayed graphically on the sheet. This can be different than the name of the schedule used in the GUI.
1. **IsOnSheet**: `TRUE` if the drawing schedule appears on a sheet, otherwise `FALSE`. 
1. **ColumnHeaders**: Drawing schedule column headers. Headers with groupings are formatted with a "|" delimiter like: "Group Text|Column Header Text". Column headers and groupings are what are displayed graphically, and are not required to match the `ParameterName` containing the data.
1. **ParameterName**: Name of the parameter that stores the data. The parameter name can be different than what `ColumnHeaders` displays on the sheet. 
1. **IsShared**: `TRUE` if the `Parameter Name` is stored with a parameter with a GUID (in Revit, a "Shared Parameter"), and `FALSE` if not. 
1. **SharedParameterGUID**: GUID of the parameter (in Revit, the `Shared Parameter` GUID)
1. **ColumnValues**: All values in the column separated by the delimiter "|". Empty values result in a consecutive delimiter "||".
1. **FileHashID**: SHA256 hash of file, used to uniquely ID each file independent of filename, timestamp, and path. 
1. **Filename**: File name with file extension
1. **Filepath**: File name with path

## Example
[FINISH DOOR SCHEDULE EXAMPLE]

## Instructions To Extract Revit Schedule Data
1. [finish]
1. [finish]
1. [finish]

## To Contribute Code
1. [finish]
1. [finish]
1. [finish]

## License
[finish]
