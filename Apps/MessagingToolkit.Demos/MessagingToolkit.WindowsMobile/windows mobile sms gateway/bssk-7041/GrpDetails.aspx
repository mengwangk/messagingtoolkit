<%@ Page  MasterPageFile="~/MasterPage.master" Title="GrpDetails"%>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>GrpDetails</h1>
<asp:GridView runat="server" DataSourceID="GrpDetailsDataSource" ID="GrpDetailsGridView" AllowSorting="True" EmptyDataText="No records were returned." AutoGenerateColumns="False" AllowPaging="True" DataKeyNames="GrpDetailId"><Columns>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
<asp:TemplateField SortExpression="Groupid" HeaderText="Groupid"><EditItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllGrpMains" ID="GrpMain_GroupidDataSource" DataObjectTypeName="bulksms.GrpMain" UpdateMethod="Update" TypeName="bulksms.GrpMain" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="GrpMain_GroupidList" DataTextField="Groupid" DataValueField="Groupid" SelectedValue='<%# Bind("Groupid") %>' DataSourceID="GrpMain_GroupidDataSource" >
</asp:DropDownList>
</EditItemTemplate>
<ItemTemplate>
<asp:HyperLink ID="GroupidLink" runat="server" Text='<%# Eval("Groupid") %>' NavigateUrl='<%# Eval("Groupid", "~/GrpMainDetail.aspx?Groupid={0}")%>' />
</ItemTemplate>
</asp:TemplateField>
<asp:BoundField DataField="Mobile" SortExpression="Mobile" HeaderText="Mobile"></asp:BoundField>
<asp:BoundField DataField="Name" SortExpression="Name" HeaderText="Name"></asp:BoundField>
<asp:BoundField DataField="Detail" SortExpression="Detail" HeaderText="Detail"></asp:BoundField>
<asp:BoundField DataField="Picked" SortExpression="Picked" HeaderText="Picked"></asp:BoundField>
<asp:BoundField ReadOnly="True" DataField="GrpDetailId" InsertVisible="False" SortExpression="GrpDetailId" HeaderText="GrpDetailId"></asp:BoundField>
<asp:HyperLinkField Text="View Details" DataNavigateUrlFormatString="~/GrpDetailDetail.aspx?GrpDetailId={0}" DataNavigateUrlFields="GrpDetailId"></asp:HyperLinkField>
</Columns>
</asp:GridView>

<asp:ObjectDataSource runat="server" SelectCountMethod="GetGrpDetailsCount" DeleteMethod="Delete" ConflictDetection="CompareAllValues" SortParameterName="sortExpression" SelectMethod="GetGrpDetails" ID="GrpDetailsDataSource" DataObjectTypeName="bulksms.GrpDetail" EnablePaging="True" UpdateMethod="Update" TypeName="bulksms.GrpDetail" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter Name="tableName" QueryStringField="Table"></asp:QueryStringParameter>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="GrpDetails_Groupid" QueryStringField="GrpDetails_Groupid"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

        <div class="link">
        <asp:HyperLink runat="server" CssClass="button" ID="NewRecordLink" NavigateUrl="~/NewGrpDetail.aspx">Create New GrpDetail</asp:HyperLink>

        </div>    
    </div>
</asp:Content>
