<%@ Page  MasterPageFile="~/MasterPage.master" Title="Countries"%>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>Countries</h1>
<asp:GridView runat="server" DataSourceID="CountriesDataSource" ID="CountriesGridView" AllowSorting="True" EmptyDataText="No records were returned." AutoGenerateColumns="False" AllowPaging="True" DataKeyNames="Code"><Columns>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
<asp:BoundField ReadOnly="True" DataField="Code" SortExpression="Code" HeaderText="Code"></asp:BoundField>
<asp:BoundField DataField="Content" SortExpression="Content" HeaderText="Content"></asp:BoundField>
<asp:HyperLinkField Text="View Operators" DataNavigateUrlFormatString="~/Operators.aspx?Table=Country_Operators&amp;Operators_Code={0}" DataNavigateUrlFields="Code,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View Details" DataNavigateUrlFormatString="~/CountryDetail.aspx?Code={0}" DataNavigateUrlFields="Code"></asp:HyperLinkField>
</Columns>
</asp:GridView>

<asp:ObjectDataSource runat="server" SelectCountMethod="GetAllCountriesCount" DeleteMethod="Delete" ConflictDetection="CompareAllValues" SortParameterName="sortExpression" SelectMethod="GetAllCountries" ID="CountriesDataSource" DataObjectTypeName="bulksms.Country" EnablePaging="True" UpdateMethod="Update" TypeName="bulksms.Country" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

        <div class="link">
        <asp:HyperLink runat="server" CssClass="button" ID="NewRecordLink" NavigateUrl="~/NewCountry.aspx">Create New Country</asp:HyperLink>

        </div>    
    </div>
</asp:Content>
