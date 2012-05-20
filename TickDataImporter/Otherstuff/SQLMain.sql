--delete from TickData.dbo.tblImported
--delete from TickData.dbo.tblTickData

--truncate table  TickData.dbo.tblImported
--truncate table  TickData.dbo.tblTickData


 --BULK INSERT [TickData].[dbo].[tblTickData] FROM 'C:\Temp\Zip\RawFiles\ADH87_1987_01_13.txt' 
	--WITH 
	--(
	--	FORMATFILE = 'C:\Users\Willi\Documents\visual studio 2010\Projects\TickDataToDB\TickDataImporter\Otherstuff\bulkFormat.fmt'
	--	--FIELDTERMINATOR = ',',
	--	--ROWTERMINATOR = '\n
	--)

--bcp [TickData].[dbo].[tblTickData] format nul -f myTestSkipCol_Default.xml -c -x -T

-- bcp [TickData].[dbo].[tblTickData] format nul -f myTestSkipCol_Default.fmt -c -T -S MWA\SQLEXPRESS
-- bcp [TickData].[dbo].[tblTickData] format nul -f bulkFormat.xml -c -x -T -S MWA\SQLEXPRESS