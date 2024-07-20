<%@ Page  MasterPageFile="~/MasterPage.master" Title="PriorityGroup Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>PriorityGroup Detail</h1>      

<asp:DetailsView runat="server" DataSourceID="PriorityGroupsDataSource" EmptyDataText="The requested record was not found." ID="PriorityGroupsDetailsView" AutoGenerateRows="False" DataKeyNames="PriorityGroupId"><Fields>
<asp:BoundField DataField="GroupName" InsertVisible="False" HeaderText="GroupName"></asp:BoundField>
<asp:BoundField ReadOnly="True" DataField="PriorityGroupId" InsertVisible="False" HeaderText="PriorityGroupId"></asp:BoundField>
<asp:HyperLinkField Text="View Users" DataNavigateUrlFormatString="~/Users.aspx?Table=PriorityGroup_Users&amp;Users_PriorityGroupId={0}" DataNavigateUrlFields="PriorityGroupId,"></asp:HyperLinkField>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetPriorityGroup" ID="PriorityGroupsDataSource" DataObjectTypeName="bulksms.PriorityGroup" UpdateMethod="Update" TypeName="bulksms.PriorityGroup" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="PriorityGroupId" QueryStringField="PriorityGroupId"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>
