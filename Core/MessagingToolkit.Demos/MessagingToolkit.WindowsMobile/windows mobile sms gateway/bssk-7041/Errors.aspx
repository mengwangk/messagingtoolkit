<%@ Page  MasterPageFile="~/MasterPage.master" Title="Errors"%>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>Errors</h1>
<asp:GridView runat="server" DataSourceID="ErrorsDataSource" ID="ErrorsGridView" AllowSorting="True" EmptyDataText="No records were returned." AutoGenerateColumns="False" AllowPaging="True" DataKeyNames="Id"><Columns>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
<asp:BoundField ReadOnly="True" DataField="Id" InsertVisible="False" SortExpression="Id" HeaderText="Id"></asp:BoundField>
<asp:BoundField DataField="Username" SortExpression="Username" HeaderText="Username"></asp:BoundField>
<asp:BoundField DataField="Date" SortExpression="Date" HeaderText="Date"></asp:BoundField>
<asp:BoundField DataField="Num" SortExpression="Num" HeaderText="Num"></asp:BoundField>
<asp:BoundField DataField="Msg" SortExpression="Msg" HeaderText="Msg"></asp:BoundField>
<asp:BoundField DataField="File" SortExpression="File" HeaderText="File"></asp:BoundField>
<asp:BoundField DataField="Line" SortExpression="Line" HeaderText="Line"></asp:BoundField>
<asp:HyperLinkField Text="View Details" DataNavigateUrlFormatString="~/ErrorDetail.aspx?Id={0}" DataNavigateUrlFields="Id"></asp:HyperLinkField>
</Columns>
</asp:GridView>

<asp:ObjectDataSource runat="server" SelectCountMethod="GetAllErrorsCount" DeleteMethod="Delete" ConflictDetection="CompareAllValues" SortParameterName="sortExpression" SelectMethod="GetAllErrors" ID="ErrorsDataSource" DataObjectTypeName="bulksms.Error" EnablePaging="True" UpdateMethod="Update" TypeName="bulksms.Error" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

        <div class="link">
        <asp:HyperLink runat="server" CssClass="button" ID="NewRecordLink" NavigateUrl="~/NewError.aspx">Create New Error</asp:HyperLink>

        </div>    
    </div>
</asp:Content>
