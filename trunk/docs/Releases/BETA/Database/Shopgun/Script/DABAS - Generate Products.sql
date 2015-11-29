SELECT REPLACE(FpCu.CUEANNR,"0", "", 1,1,) AS GTIN, TVerkare.TVNAMN AS Owner, VMarken.VMVARUMARKE AS Brand, FpCu.CUBENAM AS Produkt, 
 CUVARDE AS Quantity,  CUSORT AS [Quantity unit],
FpCu.T0168_URSPRUNGSLAND AS Orgin, FpCu.PRINGREDIENS AS Ingredients
"" as Labels, "" as Allergen, "" as Product-category, "" as Description, "Ola Thorsen" as [Code Hunter], "ola.thorsen@consumentor.org" as E-mail, "Consumentor" as Organisation, 
"" as 
FROM FpCu INNER JOIN (TVerkare INNER JOIN VMarken ON TVerkare.TVIDENT = VMarken.TVIDENT) ON FpCu.VMIDENT = VMarken.VMIDENT;


OWNERS
SELECT TVerkare.TVNAMN AS Owner, Uppglnre.ULPOSTADR AS Address, TVerkare.TVPOSTNR AS Postcode, TVerkare.TVPOSTORT AS City, Uppglnre.ULTELNR AS Telephone, TVerkare.TVLAND AS Country, Uppglnre.ULHOMEPAGE AS Homepage, Uppglnre.ULEPOST AS [E-mail], "Ola Thorsen" AS [Code Hunter], "Consumentor" AS Organisation, FpCu.CULANSDATUM AS [Date]
FROM Uppglnre INNER JOIN (FpCu INNER JOIN (TVerkare INNER JOIN VMarken ON TVerkare.TVIDENT = VMarken.TVIDENT) ON FpCu.VMIDENT = VMarken.VMIDENT) ON Uppglnre.ULIDENT = TVerkare.ULIDENT
GROUP BY TVerkare.TVNAMN, Uppglnre.ULPOSTADR, TVerkare.TVPOSTNR, TVerkare.TVPOSTORT, Uppglnre.ULTELNR, TVerkare.TVLAND, Uppglnre.ULHOMEPAGE, Uppglnre.ULEPOST, "Ola Thorsen", "Consumentor", FpCu.CULANSDATUM;
