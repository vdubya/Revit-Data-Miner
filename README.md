## Quick Start
1. Download and open [ScheduleCSVParser-v0.1.rvt](https://github.com/vdubya/Revit-Schedule-CSV-Parser/releases/download/v0.1/ScheduleCSVParser-v0.1.rvt) with Revit.
1. Select a)source folder with RVT files to scan (the macro includes all subfolders) and b)CSV export destination folder.
1. Create a new Github Issue and attach the exported CSV to contribute to crowdsourced data analysis.

## About
* The Revit Schedule CSV Parser is a Revit utility that performs a batch extraction to CSV of all drawing schedules from all Revit files in a folder and subfolders. 
* A key aspect of this approach is to rely on drawing schedules as a source of data that a)is legally exisitng quality controlled data that  
an existing bulk extration of quality controlled data without having to 
* Originally developed by Donovan Justice and Randall Stevens of [AVAIL](https://www.getavail.com/About) in collaboration with Van Woods as part of the Building Content Summit presentation "[Identifying Data Patterns from Professional Practice](https://github.com/vdubya/Revit-Schedule-Miner/raw/master/BiLT_2019-Data_Patterns_From_Professional_Practice.pdf)." 
* Licensed an open source license from [AVAIL](https://www.getavail.com/About).
* Users are encouraged to develop derivatives, enhancements, fixes, documentation, and participate in the submission of crowdsourced data for industry-wide analysis. 
* Please submit improvements. To encourage maximum participation, growth, and innovation, users are requested (but not required) to submit Pull Requests to incorporate improvements to the source. 
* CSV was chosen as a universally accessible format, and commonly used in many open source data science and machine learning tools. 
* Please contact Van Woods or Randall Stevens if you are interested in participating. See Contribution Guidelines [FINISH]. 

## Drawing Schedule Data Extraction Schema
The following data is extracted in the following comma delimited format with quote wrapped strings, with one CSV row for each drawing schedule column: 
~~~~
"Filename","Schedule Title","IsOnSheet","Column Headers","Parameter Name","Is Shared","Shared Parameter Guid","File Hash","Filepath","Column Values"
~~~~
1. **Filename**: File name with file extension.
1. **Schedule Title**: Title of drawing schedule if displayed graphically on the sheet. This can be different than the name of the 
1. **IsOnSheet**: `True` if the drawing schedule appears on a sheet, otherwise `False`. 
1. **Column Headers**: Drawing schedule column headers. Headers with groupings are formatted with a "|" delimiter like: "Group Text|Column Header Text". Column headers and groupings are what are displayed graphically, and are not required to match the `ParameterName` containing the data.
1. **Parameter Name**: Name of the parameter that stores the data. The parameter name can be different than what `ColumnHeaders` displays on the sheet. 
1. **Is Shared**: `True` if the `Parameter Name` is stored with a parameter with a GUID (in Revit, a "Shared Parameter"), and `False` if not. 
1. **Shared Parameter Guid**: GUID of the parameter (in Revit, the `Shared Parameter` GUID).
1. **File Hash**: SHA256 hash of file, used to uniquely ID each file independent of filename, timestamp, and path. 
1. **Filepath**: File name with path.
1. **Column Values**: All values in the column separated by the delimiter "|". Delimiter only between values, not at beginning and end. An empty column results in "" not "||".
