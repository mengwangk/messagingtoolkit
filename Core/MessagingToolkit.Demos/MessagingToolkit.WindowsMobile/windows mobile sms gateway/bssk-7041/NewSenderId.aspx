<%@ Page  MasterPageFile="~/MasterPage.master" Title="New SenderId" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>New SenderId</h1>      

<asp:DetailsView runat="server" DefaultMode="Insert" PageIndex="-1" DataSourceID="SenderIdsDataSource" EmptyDataText="The requested record was not found." ID="SenderIdsDetailsView" AutoGenerateRows="False" DataKeyNames="Tableid"><Fields>
<asp:BoundField ReadOnly="True" DataField="Tableid" HeaderText="Tableid"></asp:BoundField>
<asp:TemplateField SortExpression="Username" HeaderText="Username"><InsertItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllUsers" ID="User_UsernameDataSource" DataObjectTypeName="bulksms.User" UpdateMethod="Update" TypeName="bulksms.User" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="User_UsernameList" DataTextField="Username" DataValueField="Username" SelectedValue='<%# Bind("Username") %>' DataSourceID="User_UsernameDataSource" AppendDataBoundItems="true" >
<asp:ListItem Text="<null>" Value="" />
</asp:DropDownList>
</InsertItemTemplate>
</asp:TemplateField>
<asp:BoundField DataField="Senderid" HeaderText="Senderid"></asp:BoundField>
<asp:HyperLinkField Text="View MsgMains" DataNavigateUrlFormatString="~/MsgMains.aspx?Table=SenderId_MsgMains&amp;MsgMains_Tableid={0}" DataNavigateUrlFields="Tableid,"></asp:HyperLinkField>
<asp:CommandField CancelText="Clear Values" ShowInsertButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetSenderId" ID="SenderIdsDataSource" DataObjectTypeName="bulksms.SenderId" UpdateMethod="Update" TypeName="bulksms.SenderId" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="Tableid" QueryStringField="Tableid"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>
