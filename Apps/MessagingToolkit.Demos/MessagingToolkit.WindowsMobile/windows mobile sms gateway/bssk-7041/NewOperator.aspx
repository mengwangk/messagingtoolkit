<%@ Page  MasterPageFile="~/MasterPage.master" Title="New Operator" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>New Operator</h1>      

<asp:DetailsView runat="server" DefaultMode="Insert" PageIndex="-1" DataSourceID="OperatorsDataSource" EmptyDataText="The requested record was not found." ID="OperatorsDetailsView" AutoGenerateRows="False" DataKeyNames="OpCode"><Fields>
<asp:BoundField ReadOnly="True" DataField="OpCode" HeaderText="OpCode"></asp:BoundField>
<asp:TemplateField SortExpression="Code" HeaderText="Code"><InsertItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllCountries" ID="Country_CodeDataSource" DataObjectTypeName="bulksms.Country" UpdateMethod="Update" TypeName="bulksms.Country" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="Country_CodeList" DataTextField="Code" DataValueField="Code" SelectedValue='<%# Bind("Code") %>' DataSourceID="Country_CodeDataSource" AppendDataBoundItems="true" >
<asp:ListItem Text="<null>" Value="" />
</asp:DropDownList>
</InsertItemTemplate>
</asp:TemplateField>
<asp:BoundField DataField="OpName" HeaderText="OpName"></asp:BoundField>
<asp:CommandField CancelText="Clear Values" ShowInsertButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetOperator" ID="OperatorsDataSource" DataObjectTypeName="bulksms.Operator" UpdateMethod="Update" TypeName="bulksms.Operator" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="OpCode" QueryStringField="OpCode"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>
