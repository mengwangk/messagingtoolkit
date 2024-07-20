<%@ Page  MasterPageFile="~/MasterPage.master" Title="MsgTemplates Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>MsgTemplates Detail</h1>      

<asp:DetailsView runat="server" DataSourceID="MsgTemplatesDataSource" EmptyDataText="The requested record was not found." ID="MsgTemplatesDetailsView" AutoGenerateRows="False" DataKeyNames="Tempid"><Fields>
<asp:BoundField ReadOnly="True" DataField="Tempid" InsertVisible="False" HeaderText="Tempid"></asp:BoundField>
<asp:BoundField DataField="Userid" InsertVisible="False" HeaderText="Userid"></asp:BoundField>
<asp:BoundField DataField="Template" InsertVisible="False" HeaderText="Template"></asp:BoundField>
<asp:BoundField DataField="MobileColumn" InsertVisible="False" HeaderText="MobileColumn"></asp:BoundField>
<asp:BoundField DataField="Type" InsertVisible="False" HeaderText="Type"></asp:BoundField>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetMsgTemplates" ID="MsgTemplatesDataSource" DataObjectTypeName="bulksms.MsgTemplates" UpdateMethod="Update" TypeName="bulksms.MsgTemplates" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="Tempid" QueryStringField="Tempid"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>
