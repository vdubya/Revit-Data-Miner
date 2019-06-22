## About
The Revit Schedule CSV Parser is a Revit utility that performs a batch extraction to CSV of all Revit schedules from all Revit files in a folder and subfolders. It was developed by Donovan Justice and Randall Stevens of [AVAIL](https://www.getavail.com/About) as part of the Building Content Summit presentation "Identifying Data Patterns from Professional Practice" by Van Woods. With the permissive MIT open source license from AVAIL, derivatives, enhancments, fixes, documentation, and participation in the submission of crowd sourced data for industry-wide analysis is encouraged.

## Data Extracted
The following data is extracted in the following comma delimited format with quote wrapped strings: 
~~~~csharp
"Filename","File Hash","Filepath","Schedule Title","Column Headers","Parameter Name","Is Shared","Shared Parameter GUID","Field Type","Column Values"
~~~~

1. **Filename**: Filename. 
1. **FileHashID**: SHA256 hash of file, used to uniquely ID each file independent of filename, timestamp, and path. 
1. **Filepath**: Filename with path
1. **ScheduleTitle**: Title of drawing schedule if displayed graphically on the sheet. This can be different than the name of the schedule used in the GUI.
1. **IsOnSheet**: `TRUE` if the drawing schedule appears on a sheet, otherwise `FALSE`. 
1. **ColumnHeaders**: Drawing schedule column headers. Headers with groupings are formatted with a "|" delimiter like: "Group Text|Column Header Text". Column headers and groupings are what are displayed graphically, and are not required to match the `ParameterName` containing the data.
1. **ParameterName**: Name of the parameter that stores the data. The parameter name can be different than what `ColumnHeaders` displays on the sheet. 
1. **IsShared**: `TRUE` if the `Parameter Name` is stored with a parameter with a GUID (in Revit, a "Shared Parameter"), and `FALSE` if not. 
1. **SharedParameterGUID**: GUID of the parameter (in Revit, the `Shared Parameter` GUID)
1. **ColumnValues**: All values in the column separated by the delimiter "|". Empty values result in a consecutive delimiter "||".

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
