<%@ Page  MasterPageFile="~/MasterPage.master" Title="Payment Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>Payment Detail</h1>      

<asp:DetailsView runat="server" DataSourceID="PaymentsDataSource" EmptyDataText="The requested record was not found." ID="PaymentsDetailsView" AutoGenerateRows="False" DataKeyNames="PaymentID"><Fields>
<asp:BoundField ReadOnly="True" DataField="PaymentID" InsertVisible="False" HeaderText="PaymentID"></asp:BoundField>
<asp:BoundField DataField="ResultCode" InsertVisible="False" HeaderText="ResultCode"></asp:BoundField>
<asp:BoundField DataField="AuthCode" InsertVisible="False" HeaderText="AuthCode"></asp:BoundField>
<asp:BoundField DataField="TranID" InsertVisible="False" HeaderText="TranID"></asp:BoundField>
<asp:BoundField DataField="PostDate" InsertVisible="False" HeaderText="PostDate"></asp:BoundField>
<asp:BoundField DataField="TrackID" InsertVisible="False" HeaderText="TrackID"></asp:BoundField>
<asp:BoundField DataField="Udf2" InsertVisible="False" HeaderText="Udf2"></asp:BoundField>
<asp:BoundField DataField="CurrentTime" InsertVisible="False" HeaderText="CurrentTime"></asp:BoundField>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetPayment" ID="PaymentsDataSource" DataObjectTypeName="bulksms.Payment" UpdateMethod="Update" TypeName="bulksms.Payment" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="PaymentID" QueryStringField="PaymentID"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>
