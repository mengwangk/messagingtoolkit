<%@ Page  MasterPageFile="~/MasterPage.master" Title="SenderIds"%>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>SenderIds</h1>
<asp:GridView runat="server" DataSourceID="SenderIdsDataSource" ID="SenderIdsGridView" AllowSorting="True" EmptyDataText="No records were returned." AutoGenerateColumns="False" AllowPaging="True" DataKeyNames="Tableid"><Columns>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
<asp:BoundField ReadOnly="True" DataField="Tableid" SortExpression="Tableid" HeaderText="Tableid"></asp:BoundField>
<asp:TemplateField SortExpression="Username" HeaderText="Username"><EditItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllUsers" ID="User_UsernameDataSource" DataObjectTypeName="bulksms.User" UpdateMethod="Update" TypeName="bulksms.User" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="User_UsernameList" DataTextField="Username" DataValueField="Username" SelectedValue='<%# Bind("Username") %>' DataSourceID="User_UsernameDataSource" AppendDataBoundItems="true" >
<asp:ListItem Text="<null>" Value="" />
</asp:DropDownList>
</EditItemTemplate>
<ItemTemplate>
<asp:HyperLink ID="UsernameLink" runat="server" Text='<%# Eval("Username") %>' NavigateUrl='<%# Eval("Username", "~/UserDetail.aspx?Username={0}")%>' />
</ItemTemplate>
</asp:TemplateField>
<asp:BoundField DataField="Senderid" SortExpression="Senderid" HeaderText="Senderid"></asp:BoundField>
<asp:HyperLinkField Text="View MsgMains" DataNavigateUrlFormatString="~/MsgMains.aspx?Table=SenderId_MsgMains&amp;MsgMains_Tableid={0}" DataNavigateUrlFields="Tableid,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View Details" DataNavigateUrlFormatString="~/SenderIdDetail.aspx?Tableid={0}" DataNavigateUrlFields="Tableid"></asp:HyperLinkField>
</Columns>
</asp:GridView>

<asp:ObjectDataSource runat="server" SelectCountMethod="GetSenderIdsCount" DeleteMethod="Delete" ConflictDetection="CompareAllValues" SortParameterName="sortExpression" SelectMethod="GetSenderIds" ID="SenderIdsDataSource" DataObjectTypeName="bulksms.SenderId" EnablePaging="True" UpdateMethod="Update" TypeName="bulksms.SenderId" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter Name="tableName" QueryStringField="Table"></asp:QueryStringParameter>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="SenderIds_Username" QueryStringField="SenderIds_Username"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

        <div class="link">
        <asp:HyperLink runat="server" CssClass="button" ID="NewRecordLink" NavigateUrl="~/NewSenderId.aspx">Create New SenderId</asp:HyperLink>

        </div>    
    </div>
</asp:Content>
