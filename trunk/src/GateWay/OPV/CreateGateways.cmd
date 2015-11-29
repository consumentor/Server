REM ***************************************************************************
REM ** Create Gateways 
REM ** This script creates gateways with the help from the application wscf.exe
REM ** For options run wscf.exe /?
REM ** The script requires that the webservices are up and running
REM ***************************************************************************
CD Opv

..\..\..\tools\wscf\wscf.exe /nologo /out:ProductSearchWebService.cs /overwrite /multiplefiles /interface /genericlist /properties /namespace:Consumentor.ShopGun.Gateway.Opv http://www.mediabanken.se/WS/Consumentor/ProductService.asmx?wsdl
..\FixGatewayNaming.vbs ProductSearchWebService.cs IProductService.cs ProductService ProductSearchWebServiceGateway
..\FixGatewayInheritance.vbs ProductSearchWebServiceGateway
..\FixGatewayUrl.vbs ProductSearchWebServiceGateway http://www.mediabanken.se/WS/Consumentor/ProductService.asmx

..\FixGateWayClassNaming.vbs
CD..