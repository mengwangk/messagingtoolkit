<%@ Page  MasterPageFile="~/MasterPage.master" Title="Payments"%>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>Payments</h1>
<asp:GridView runat="server" DataSourceID="PaymentsDataSource" ID="PaymentsGridView" AllowSorting="True" EmptyDataText="No records were returned." AutoGenerateColumns="False" AllowPaging="True" DataKeyNames="PaymentID"><Columns>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
<asp:BoundField ReadOnly="True" DataField="PaymentID" SortExpression="PaymentID" HeaderText="PaymentID"></asp:BoundField>
<asp:BoundField DataField="ResultCode" SortExpression="ResultCode" HeaderText="ResultCode"></asp:BoundField>
<asp:BoundField DataField="AuthCode" SortExpression="AuthCode" HeaderText="AuthCode"></asp:BoundField>
<asp:BoundField DataField="TranID" SortExpression="TranID" HeaderText="TranID"></asp:BoundField>
<asp:BoundField DataField="PostDate" SortExpression="PostDate" HeaderText="PostDate"></asp:BoundField>
<asp:BoundField DataField="TrackID" SortExpression="TrackID" HeaderText="TrackID"></asp:BoundField>
<asp:BoundField DataField="Udf2" SortExpression="Udf2" HeaderText="Udf2"></asp:BoundField>
<asp:BoundField DataField="CurrentTime" SortExpression="CurrentTime" HeaderText="CurrentTime"></asp:BoundField>
<asp:HyperLinkField Text="View Details" DataNavigateUrlFormatString="~/PaymentDetail.aspx?PaymentID={0}" DataNavigateUrlFields="PaymentID"></asp:HyperLinkField>
</Columns>
</asp:GridView>

<asp:ObjectDataSource runat="server" SelectCountMethod="GetAllPaymentsCount" DeleteMethod="Delete" ConflictDetection="CompareAllValues" SortParameterName="sortExpression" SelectMethod="GetAllPayments" ID="PaymentsDataSource" DataObjectTypeName="bulksms.Payment" EnablePaging="True" UpdateMethod="Update" TypeName="bulksms.Payment" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

        <div class="link">
        <asp:HyperLink runat="server" CssClass="button" ID="NewRecordLink" NavigateUrl="~/NewPayment.aspx">Create New Payment</asp:HyperLink>

        </div>    
    </div>
</asp:Content>
