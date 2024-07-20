<%@ Page  MasterPageFile="~/MasterPage.master" Title="News"%>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>News</h1>
<asp:GridView runat="server" DataSourceID="NewsDataSource" ID="NewsGridView" AllowSorting="True" EmptyDataText="No records were returned." AutoGenerateColumns="False" AllowPaging="True" DataKeyNames="NewsId"><Columns>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
<asp:BoundField ReadOnly="True" DataField="NewsId" SortExpression="NewsId" HeaderText="NewsId"></asp:BoundField>
<asp:BoundField DataField="Title" SortExpression="Title" HeaderText="Title"></asp:BoundField>
<asp:BoundField DataField="News" SortExpression="News" HeaderText="News"></asp:BoundField>
<asp:BoundField DataField="NewsDate" SortExpression="NewsDate" HeaderText="NewsDate"></asp:BoundField>
<asp:HyperLinkField Text="View Details" DataNavigateUrlFormatString="~/NewDetail.aspx?NewsId={0}" DataNavigateUrlFields="NewsId"></asp:HyperLinkField>
</Columns>
</asp:GridView>

<asp:ObjectDataSource runat="server" SelectCountMethod="GetAllNewsCount" DeleteMethod="Delete" ConflictDetection="CompareAllValues" SortParameterName="sortExpression" SelectMethod="GetAllNews" ID="NewsDataSource" DataObjectTypeName="bulksms.New" EnablePaging="True" UpdateMethod="Update" TypeName="bulksms.New" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

        <div class="link">
        <asp:HyperLink runat="server" CssClass="button" ID="NewRecordLink" NavigateUrl="~/NewNew.aspx">Create New New</asp:HyperLink>

        </div>    
    </div>
</asp:Content>
