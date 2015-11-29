<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Consumentor.ShopGun.Domain.AdviceBase>" %>

<link rel="stylesheet" href="<%=Url.Content("~/Content/css/AdviceScreen.css")%>" type="text/css" media="all" />

<div class="center">
    <span class="moep">Brought to you by:</span>
    <a href="http://www.shopgun.se">
        <img style="margin-top:-20px" alt="Shopgun" src="../../Content/images/shopgun_trans-150x119.png" />
    </a>
</div>

<div>
<% if (Model == null)
   { %>
    <%= ViewData["Message"]%>       
<% }
   else
   {
       var semaphoreColor = Model.Semaphore.ColorName.ToLower(); 
%>       
    <div id="adviceScreen" class="center">
        <div id="adviceHeader">
            <div id="ribbon" class="<%=semaphoreColor %>">
                <%--<p><%=Html.Encode(Model.ItemName)%></p>--%>
            </div>
            <div id="adviceTag">
                <%= Html.Encode(Model.TagName) %>
            </div>
        </div>
        <div id="adviceContent">
            <div id="adviceLabel"><%= Model.Label %></div>
            <div id="advisor"><%= Html.Encode(Model.Mentor.MentorName) %></div>
            <div id="adviceText"><%= Model.Advice %></div>
        </div>
        <div id="adviceFooter" class="<%=semaphoreColor %>"></div>
    </div>
</div>
<%
   }%>


