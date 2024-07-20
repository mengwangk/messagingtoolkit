<%@ Page  MasterPageFile="~/MasterPage.master" Title="New MsgTemplates" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>New MsgTemplates</h1>      

<asp:DetailsView runat="server" DefaultMode="Insert" PageIndex="-1" DataSourceID="MsgTemplatesDataSource" EmptyDataText="The requested record was not found." ID="MsgTemplatesDetailsView" AutoGenerateRows="False" DataKeyNames="Tempid"><Fields>
<asp:BoundField ReadOnly="True" DataField="Tempid" HeaderText="Tempid"></asp:BoundField>
<asp:BoundField DataField="Userid" HeaderText="Userid"></asp:BoundField>
<asp:BoundField DataField="Template" HeaderText="Template"></asp:BoundField>
<asp:BoundField DataField="MobileColumn" HeaderText="MobileColumn"></asp:BoundField>
<asp:BoundField DataField="Type" HeaderText="Type"></asp:BoundField>
<asp:CommandField CancelText="Clear Values" ShowInsertButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetMsgTemplates" ID="MsgTemplatesDataSource" DataObjectTypeName="bulksms.MsgTemplates" UpdateMethod="Update" TypeName="bulksms.MsgTemplates" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="Tempid" QueryStringField="Tempid"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>
