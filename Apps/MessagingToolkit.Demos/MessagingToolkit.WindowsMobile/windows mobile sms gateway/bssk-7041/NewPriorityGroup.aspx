<%@ Page  MasterPageFile="~/MasterPage.master" Title="New PriorityGroup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>New PriorityGroup</h1>      

<asp:DetailsView runat="server" DefaultMode="Insert" PageIndex="-1" DataSourceID="PriorityGroupsDataSource" EmptyDataText="The requested record was not found." ID="PriorityGroupsDetailsView" AutoGenerateRows="False" DataKeyNames="PriorityGroupId"><Fields>
<asp:BoundField DataField="GroupName" HeaderText="GroupName"></asp:BoundField>
<asp:BoundField ReadOnly="True" DataField="PriorityGroupId" InsertVisible="False" HeaderText="PriorityGroupId"></asp:BoundField>
<asp:HyperLinkField Text="View Users" DataNavigateUrlFormatString="~/Users.aspx?Table=PriorityGroup_Users&amp;Users_PriorityGroupId={0}" DataNavigateUrlFields="PriorityGroupId,"></asp:HyperLinkField>
<asp:CommandField CancelText="Clear Values" ShowInsertButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetPriorityGroup" ID="PriorityGroupsDataSource" DataObjectTypeName="bulksms.PriorityGroup" UpdateMethod="Update" TypeName="bulksms.PriorityGroup" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="PriorityGroupId" QueryStringField="PriorityGroupId"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>
