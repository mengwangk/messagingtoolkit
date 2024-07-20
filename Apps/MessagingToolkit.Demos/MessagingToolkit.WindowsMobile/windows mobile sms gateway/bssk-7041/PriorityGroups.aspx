<%@ Page  MasterPageFile="~/MasterPage.master" Title="PriorityGroups"%>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>PriorityGroups</h1>
<asp:GridView runat="server" DataSourceID="PriorityGroupsDataSource" ID="PriorityGroupsGridView" AllowSorting="True" EmptyDataText="No records were returned." AutoGenerateColumns="False" AllowPaging="True" DataKeyNames="PriorityGroupId"><Columns>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
<asp:BoundField DataField="GroupName" SortExpression="GroupName" HeaderText="GroupName"></asp:BoundField>
<asp:BoundField ReadOnly="True" DataField="PriorityGroupId" InsertVisible="False" SortExpression="PriorityGroupId" HeaderText="PriorityGroupId"></asp:BoundField>
<asp:HyperLinkField Text="View Users" DataNavigateUrlFormatString="~/Users.aspx?Table=PriorityGroup_Users&amp;Users_PriorityGroupId={0}" DataNavigateUrlFields="PriorityGroupId,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View Details" DataNavigateUrlFormatString="~/PriorityGroupDetail.aspx?PriorityGroupId={0}" DataNavigateUrlFields="PriorityGroupId"></asp:HyperLinkField>
</Columns>
</asp:GridView>

<asp:ObjectDataSource runat="server" SelectCountMethod="GetAllPriorityGroupsCount" DeleteMethod="Delete" ConflictDetection="CompareAllValues" SortParameterName="sortExpression" SelectMethod="GetAllPriorityGroups" ID="PriorityGroupsDataSource" DataObjectTypeName="bulksms.PriorityGroup" EnablePaging="True" UpdateMethod="Update" TypeName="bulksms.PriorityGroup" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

        <div class="link">
        <asp:HyperLink runat="server" CssClass="button" ID="NewRecordLink" NavigateUrl="~/NewPriorityGroup.aspx">Create New PriorityGroup</asp:HyperLink>

        </div>    
    </div>
</asp:Content>
