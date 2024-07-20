<%@ Page  MasterPageFile="~/MasterPage.master" Title="GrpMains"%>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>GrpMains</h1>
<asp:GridView runat="server" DataSourceID="GrpMainsDataSource" ID="GrpMainsGridView" AllowSorting="True" EmptyDataText="No records were returned." AutoGenerateColumns="False" AllowPaging="True" DataKeyNames="Groupid"><Columns>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
<asp:TemplateField SortExpression="Username" HeaderText="Username"><EditItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllUsers" ID="User_UsernameDataSource" DataObjectTypeName="bulksms.User" UpdateMethod="Update" TypeName="bulksms.User" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="User_UsernameList" DataTextField="Username" DataValueField="Username" SelectedValue='<%# Bind("Username") %>' DataSourceID="User_UsernameDataSource" >
</asp:DropDownList>
</EditItemTemplate>
<ItemTemplate>
<asp:HyperLink ID="UsernameLink" runat="server" Text='<%# Eval("Username") %>' NavigateUrl='<%# Eval("Username", "~/UserDetail.aspx?Username={0}")%>' />
</ItemTemplate>
</asp:TemplateField>
<asp:BoundField ReadOnly="True" DataField="Groupid" SortExpression="Groupid" HeaderText="Groupid"></asp:BoundField>
<asp:BoundField DataField="Groupname" SortExpression="Groupname" HeaderText="Groupname"></asp:BoundField>
<asp:BoundField DataField="Detail" SortExpression="Detail" HeaderText="Detail"></asp:BoundField>
<asp:BoundField DataField="Grouporder" SortExpression="Grouporder" HeaderText="Grouporder"></asp:BoundField>
<asp:HyperLinkField Text="View GrpDetails" DataNavigateUrlFormatString="~/GrpDetails.aspx?Table=GrpMain_GrpDetails&amp;GrpDetails_Groupid={0}" DataNavigateUrlFields="Groupid,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View MsgDetails" DataNavigateUrlFormatString="~/MsgDetails.aspx?Table=GrpMain_MsgDetails&amp;MsgDetails_Groupid={0}" DataNavigateUrlFields="Groupid,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View MsgHistories" DataNavigateUrlFormatString="~/MsgHistories.aspx?Table=GrpMain_MsgHistories&amp;MsgHistories_Groupid={0}" DataNavigateUrlFields="Groupid,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View Details" DataNavigateUrlFormatString="~/GrpMainDetail.aspx?Groupid={0}" DataNavigateUrlFields="Groupid"></asp:HyperLinkField>
</Columns>
</asp:GridView>

<asp:ObjectDataSource runat="server" SelectCountMethod="GetGrpMainsCount" DeleteMethod="Delete" ConflictDetection="CompareAllValues" SortParameterName="sortExpression" SelectMethod="GetGrpMains" ID="GrpMainsDataSource" DataObjectTypeName="bulksms.GrpMain" EnablePaging="True" UpdateMethod="Update" TypeName="bulksms.GrpMain" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter Name="tableName" QueryStringField="Table"></asp:QueryStringParameter>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="GrpMains_Username" QueryStringField="GrpMains_Username"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

        <div class="link">
        <asp:HyperLink runat="server" CssClass="button" ID="NewRecordLink" NavigateUrl="~/NewGrpMain.aspx">Create New GrpMain</asp:HyperLink>

        </div>    
    </div>
</asp:Content>
