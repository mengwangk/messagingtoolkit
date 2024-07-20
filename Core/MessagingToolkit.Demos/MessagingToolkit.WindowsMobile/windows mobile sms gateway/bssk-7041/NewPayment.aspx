<%@ Page  MasterPageFile="~/MasterPage.master" Title="New Payment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>New Payment</h1>      

<asp:DetailsView runat="server" DefaultMode="Insert" PageIndex="-1" DataSourceID="PaymentsDataSource" EmptyDataText="The requested record was not found." ID="PaymentsDetailsView" AutoGenerateRows="False" DataKeyNames="PaymentID"><Fields>
<asp:BoundField ReadOnly="True" DataField="PaymentID" HeaderText="PaymentID"></asp:BoundField>
<asp:BoundField DataField="ResultCode" HeaderText="ResultCode"></asp:BoundField>
<asp:BoundField DataField="AuthCode" HeaderText="AuthCode"></asp:BoundField>
<asp:BoundField DataField="TranID" HeaderText="TranID"></asp:BoundField>
<asp:BoundField DataField="PostDate" HeaderText="PostDate"></asp:BoundField>
<asp:BoundField DataField="TrackID" HeaderText="TrackID"></asp:BoundField>
<asp:BoundField DataField="Udf2" HeaderText="Udf2"></asp:BoundField>
<asp:BoundField DataField="CurrentTime" HeaderText="CurrentTime"></asp:BoundField>
<asp:CommandField CancelText="Clear Values" ShowInsertButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetPayment" ID="PaymentsDataSource" DataObjectTypeName="bulksms.Payment" UpdateMethod="Update" TypeName="bulksms.Payment" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="PaymentID" QueryStringField="PaymentID"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>
