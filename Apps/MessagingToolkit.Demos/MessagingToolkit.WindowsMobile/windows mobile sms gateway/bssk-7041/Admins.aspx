<%@ Page  MasterPageFile="~/MasterPage.master" Title="Admins"%>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>Admins</h1>
<asp:GridView runat="server" DataSourceID="AdminsDataSource" ID="AdminsGridView" AllowSorting="True" EmptyDataText="No records were returned." AutoGenerateColumns="False" AllowPaging="True" DataKeyNames="Username"><Columns>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
<asp:BoundField ReadOnly="True" DataField="Username" SortExpression="Username" HeaderText="Username"></asp:BoundField>
<asp:BoundField DataField="Password" SortExpression="Password" HeaderText="Password"></asp:BoundField>
<asp:BoundField DataField="Lastvisit" SortExpression="Lastvisit" HeaderText="Lastvisit"></asp:BoundField>
<asp:BoundField DataField="Sold" SortExpression="Sold" HeaderText="Sold"></asp:BoundField>
<asp:HyperLinkField Text="View Details" DataNavigateUrlFormatString="~/AdminDetail.aspx?Username={0}" DataNavigateUrlFields="Username"></asp:HyperLinkField>
</Columns>
</asp:GridView>

<asp:ObjectDataSource runat="server" SelectCountMethod="GetAllAdminsCount" DeleteMethod="Delete" ConflictDetection="CompareAllValues" SortParameterName="sortExpression" SelectMethod="GetAllAdmins" ID="AdminsDataSource" DataObjectTypeName="bulksms.Admin" EnablePaging="True" UpdateMethod="Update" TypeName="bulksms.Admin" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

        <div class="link">
        <asp:HyperLink runat="server" CssClass="button" ID="NewRecordLink" NavigateUrl="~/NewAdmin.aspx">Create New Admin</asp:HyperLink>

        </div>    
    </div>
</asp:Content>
