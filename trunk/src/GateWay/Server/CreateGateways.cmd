REM ***************************************************************************
REM ** Create Gateways 
REM ** This script creates gateways with the help from the application wscf.exe
REM ** For options run wscf.exe /?
REM ** The script requires that the webservices are up and running
REM ***************************************************************************
CD Server

..\..\..\tools\wscf\wscf.exe /nologo /out:ProductSearchWebService.cs /overwrite /multiplefiles /interface /genericlist /properties /namespace:Consumentor.ShopGun.Gateway.Server http://localhost:8080/Server/ProductSearchWebService/?wsdl
..\FixGatewayNaming.vbs ProductSearchWebService.cs IProductSearchWebService.cs ProductSearchWebService ProductSearchWebServiceGateway
..\FixGatewayInheritance.vbs ProductSearchWebServiceGateway
..\FixGatewayUrl.vbs ProductSearchWebServiceGateway http://localhost:8080/Server/ProductSearchWebService/

..\FixGateWayClassNaming.vbs
CD..