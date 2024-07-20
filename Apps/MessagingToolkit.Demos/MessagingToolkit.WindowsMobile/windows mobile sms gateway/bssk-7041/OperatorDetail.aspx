<%@ Page  MasterPageFile="~/MasterPage.master" Title="Operator Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>Operator Detail</h1>      

<asp:DetailsView runat="server" DataSourceID="OperatorsDataSource" EmptyDataText="The requested record was not found." ID="OperatorsDetailsView" AutoGenerateRows="False" DataKeyNames="OpCode"><Fields>
<asp:BoundField ReadOnly="True" DataField="OpCode" InsertVisible="False" HeaderText="OpCode"></asp:BoundField>
<asp:TemplateField SortExpression="Code" HeaderText="Code"><EditItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllCountries" ID="Country_CodeDataSource" DataObjectTypeName="bulksms.Country" UpdateMethod="Update" TypeName="bulksms.Country" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="Country_CodeList" DataTextField="Code" DataValueField="Code" SelectedValue='<%# Bind("Code") %>' DataSourceID="Country_CodeDataSource" AppendDataBoundItems="true" >
<asp:ListItem Text="<null>" Value="" />
</asp:DropDownList>
</EditItemTemplate>
<ItemTemplate>
<asp:HyperLink ID="CodeLink" runat="server" Text='<%# Eval("Code") %>' NavigateUrl='<%# Eval("Code", "~/CountryDetail.aspx?Code={0}")%>' />
</ItemTemplate>
</asp:TemplateField>
<asp:BoundField DataField="OpName" InsertVisible="False" HeaderText="OpName"></asp:BoundField>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetOperator" ID="OperatorsDataSource" DataObjectTypeName="bulksms.Operator" UpdateMethod="Update" TypeName="bulksms.Operator" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="OpCode" QueryStringField="OpCode"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>
