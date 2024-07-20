<%@ Page  MasterPageFile="~/MasterPage.master" Title="New Sysdiagram" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>New Sysdiagram</h1>      

<asp:DetailsView runat="server" DefaultMode="Insert" PageIndex="-1" DataSourceID="SysdiagramsDataSource" EmptyDataText="The requested record was not found." ID="SysdiagramsDetailsView" AutoGenerateRows="False" DataKeyNames="DiagramId"><Fields>
<asp:BoundField DataField="Name" HeaderText="Name"></asp:BoundField>
<asp:BoundField DataField="PrincipalId" HeaderText="PrincipalId"></asp:BoundField>
<asp:BoundField ReadOnly="True" DataField="DiagramId" InsertVisible="False" HeaderText="DiagramId"></asp:BoundField>
<asp:BoundField DataField="Version" HeaderText="Version"></asp:BoundField>
<asp:CommandField CancelText="Clear Values" ShowInsertButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetSysdiagram" ID="SysdiagramsDataSource" DataObjectTypeName="bulksms.Sysdiagram" UpdateMethod="Update" TypeName="bulksms.Sysdiagram" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="DiagramId" QueryStringField="DiagramId"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>
