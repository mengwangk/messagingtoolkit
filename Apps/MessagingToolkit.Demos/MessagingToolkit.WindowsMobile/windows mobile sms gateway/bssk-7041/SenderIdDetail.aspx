<%@ Page  MasterPageFile="~/MasterPage.master" Title="SenderId Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>SenderId Detail</h1>      

<asp:DetailsView runat="server" DataSourceID="SenderIdsDataSource" EmptyDataText="The requested record was not found." ID="SenderIdsDetailsView" AutoGenerateRows="False" DataKeyNames="Tableid"><Fields>
<asp:BoundField ReadOnly="True" DataField="Tableid" InsertVisible="False" HeaderText="Tableid"></asp:BoundField>
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
<asp:BoundField DataField="Senderid" InsertVisible="False" HeaderText="Senderid"></asp:BoundField>
<asp:HyperLinkField Text="View MsgMains" DataNavigateUrlFormatString="~/MsgMains.aspx?Table=SenderId_MsgMains&amp;MsgMains_Tableid={0}" DataNavigateUrlFields="Tableid,"></asp:HyperLinkField>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetSenderId" ID="SenderIdsDataSource" DataObjectTypeName="bulksms.SenderId" UpdateMethod="Update" TypeName="bulksms.SenderId" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="Tableid" QueryStringField="Tableid"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>
