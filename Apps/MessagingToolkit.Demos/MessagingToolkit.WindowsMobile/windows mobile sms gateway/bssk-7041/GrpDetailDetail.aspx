<%@ Page  MasterPageFile="~/MasterPage.master" Title="GrpDetail Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>GrpDetail Detail</h1>      

<asp:DetailsView runat="server" DataSourceID="GrpDetailsDataSource" EmptyDataText="The requested record was not found." ID="GrpDetailsDetailsView" AutoGenerateRows="False" DataKeyNames="GrpDetailId"><Fields>
<asp:TemplateField SortExpression="Groupid" HeaderText="Groupid"><EditItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllGrpMains" ID="GrpMain_GroupidDataSource" DataObjectTypeName="bulksms.GrpMain" UpdateMethod="Update" TypeName="bulksms.GrpMain" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="GrpMain_GroupidList" DataTextField="Groupid" DataValueField="Groupid" SelectedValue='<%# Bind("Groupid") %>' DataSourceID="GrpMain_GroupidDataSource" >
</asp:DropDownList>
</EditItemTemplate>
<ItemTemplate>
<asp:HyperLink ID="GroupidLink" runat="server" Text='<%# Eval("Groupid") %>' NavigateUrl='<%# Eval("Groupid", "~/GrpMainDetail.aspx?Groupid={0}")%>' />
</ItemTemplate>
</asp:TemplateField>
<asp:BoundField DataField="Mobile" InsertVisible="False" HeaderText="Mobile"></asp:BoundField>
<asp:BoundField DataField="Name" InsertVisible="False" HeaderText="Name"></asp:BoundField>
<asp:BoundField DataField="Detail" InsertVisible="False" HeaderText="Detail"></asp:BoundField>
<asp:BoundField DataField="Picked" InsertVisible="False" HeaderText="Picked"></asp:BoundField>
<asp:BoundField ReadOnly="True" DataField="GrpDetailId" InsertVisible="False" HeaderText="GrpDetailId"></asp:BoundField>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetGrpDetail" ID="GrpDetailsDataSource" DataObjectTypeName="bulksms.GrpDetail" UpdateMethod="Update" TypeName="bulksms.GrpDetail" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="GrpDetailId" QueryStringField="GrpDetailId"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>
