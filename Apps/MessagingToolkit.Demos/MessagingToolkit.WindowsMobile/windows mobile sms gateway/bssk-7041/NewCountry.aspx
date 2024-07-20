<%@ Page  MasterPageFile="~/MasterPage.master" Title="New Country" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>New Country</h1>      

<asp:DetailsView runat="server" DefaultMode="Insert" PageIndex="-1" DataSourceID="CountriesDataSource" EmptyDataText="The requested record was not found." ID="CountriesDetailsView" AutoGenerateRows="False" DataKeyNames="Code"><Fields>
<asp:BoundField ReadOnly="True" DataField="Code" HeaderText="Code"></asp:BoundField>
<asp:BoundField DataField="Content" HeaderText="Content"></asp:BoundField>
<asp:HyperLinkField Text="View Operators" DataNavigateUrlFormatString="~/Operators.aspx?Table=Country_Operators&amp;Operators_Code={0}" DataNavigateUrlFields="Code,"></asp:HyperLinkField>
<asp:CommandField CancelText="Clear Values" ShowInsertButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetCountry" ID="CountriesDataSource" DataObjectTypeName="bulksms.Country" UpdateMethod="Update" TypeName="bulksms.Country" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="Code" QueryStringField="Code"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>