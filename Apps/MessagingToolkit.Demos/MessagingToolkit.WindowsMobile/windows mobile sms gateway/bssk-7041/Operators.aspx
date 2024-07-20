<%@ Page  MasterPageFile="~/MasterPage.master" Title="Operators"%>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>Operators</h1>
<asp:GridView runat="server" DataSourceID="OperatorsDataSource" ID="OperatorsGridView" AllowSorting="True" EmptyDataText="No records were returned." AutoGenerateColumns="False" AllowPaging="True" DataKeyNames="OpCode"><Columns>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
<asp:BoundField ReadOnly="True" DataField="OpCode" SortExpression="OpCode" HeaderText="OpCode"></asp:BoundField>
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
<asp:BoundField DataField="OpName" SortExpression="OpName" HeaderText="OpName"></asp:BoundField>
<asp:HyperLinkField Text="View Details" DataNavigateUrlFormatString="~/OperatorDetail.aspx?OpCode={0}" DataNavigateUrlFields="OpCode"></asp:HyperLinkField>
</Columns>
</asp:GridView>

<asp:ObjectDataSource runat="server" SelectCountMethod="GetOperatorsCount" DeleteMethod="Delete" ConflictDetection="CompareAllValues" SortParameterName="sortExpression" SelectMethod="GetOperators" ID="OperatorsDataSource" DataObjectTypeName="bulksms.Operator" EnablePaging="True" UpdateMethod="Update" TypeName="bulksms.Operator" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter Name="tableName" QueryStringField="Table"></asp:QueryStringParameter>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="Operators_Code" QueryStringField="Operators_Code"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

        <div class="link">
        <asp:HyperLink runat="server" CssClass="button" ID="NewRecordLink" NavigateUrl="~/NewOperator.aspx">Create New Operator</asp:HyperLink>

        </div>    
    </div>
</asp:Content>
