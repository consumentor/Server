REM ***************************************************************************
REM ** Create Gateways 
REM ** This script creates gateways with the help from the application wscf.exe
REM ** For options run wscf.exe /?
REM ** The script requires that the webservices are up and running
REM ***************************************************************************


..\..\..\tools\wscf\wscf.exe /nologo /out:ProductSearchWebService.cs /overwrite /multiplefiles /interface /genericlist /properties /namespace:Consumentor.ShopGun.Gateway.Gepir http://gepir.gs1.se/router/router.asmx?wsdl
..\FixGatewayNaming.vbs ProductSearchWebService.cs Irouter.cs router ProductSearchWebServiceGateway
..\FixGatewayInheritance.vbs ProductSearchWebServiceGateway
..\FixGatewayUrl.vbs ProductSearchWebServiceGateway http://gepir.gs1.se/router/router.asmx

..\FixGateWayClassNaming.vbs
CD..