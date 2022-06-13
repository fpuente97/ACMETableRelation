# ACME Table Relation - ioet Exercise
### [By Fernando Puente](https://github.com/fpuente97)

Application to check how often employees coincide in pairs in the office using `C#` and `.NET 5.0 Framework`

For this exercise, I used `Regex` to validate the Input data alongside array functions and `LINQ` to manage the input data. 

##Solution

First, the application saves all the lines (without any white space) in the text file to one List of string type. After this, the application reads line by line, using Regex to check that there is no any other special character before or after the delimiter `'='` and check by the resulting array count if there was two or more of it.

Then starts to read all the days with working times and saves to one `array`. Check for the first two char in the strings to validate if they have the correct spelling by checking if coincides with one `string` insides the days array.

And to finish the data reading, the application splits the working times, convert to `DateTime` and check that the out time is not before that the in time.

After the read process finishes, it saves the data into a `EmployeeWorkedDayAndTime` type`List` which can store the `name`, `worked day` and both `out` and `in times`.

If an error had ocurred during this process, it outputs a message of which error had occur and continue with the next data item.

To start with the time comparing process to check the coincidences, the app uses `LINQ` to filter the data by day and compares one employee with the others that worked in that day. If there's any coincidence, than it saves it in a `EmployeeCompared` type `List`, which can store the pairs that are found.

After finish to check one employee, it is removed from the the list and continues with the next employee. This was necessary to not save the same coincidence twice.

And to write the output table, the app uses `LINQ` again to return one Count query and write each record to the console.

##Structure

I decided to split the main function in three methods: `checking for the file`, `reading the file` and `comparing employees`. As I found it easier to code by steps.
And created two classes to manage data through `LINQ`, as this is a similar approach as `SQL`, which is an easy way to manage data.

##How to run

Before execution, create a text file named as `InputFile.txt` with the input data in the same directory of the app.

