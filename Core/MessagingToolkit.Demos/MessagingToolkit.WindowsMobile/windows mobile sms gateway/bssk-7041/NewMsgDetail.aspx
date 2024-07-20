<%@ Page  MasterPageFile="~/MasterPage.master" Title="New MsgDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>New MsgDetail</h1>      

<asp:DetailsView runat="server" DefaultMode="Insert" PageIndex="-1" DataSourceID="MsgDetailsDataSource" EmptyDataText="The requested record was not found." ID="MsgDetailsDetailsView" AutoGenerateRows="False" DataKeyNames="MsgDetailId"><Fields>
<asp:BoundField DataField="Mobile" HeaderText="Mobile"></asp:BoundField>
<asp:TemplateField SortExpression="Groupid" HeaderText="Groupid"><InsertItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllGrpMains" ID="GrpMain_GroupidDataSource" DataObjectTypeName="bulksms.GrpMain" UpdateMethod="Update" TypeName="bulksms.GrpMain" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="GrpMain_GroupidList" DataTextField="Groupid" DataValueField="Groupid" SelectedValue='<%# Bind("Groupid") %>' DataSourceID="GrpMain_GroupidDataSource" >
</asp:DropDownList>
</InsertItemTemplate>
</asp:TemplateField>
<asp:TemplateField SortExpression="Msgid" HeaderText="Msgid"><InsertItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllMsgMains" ID="MsgMain_MsgidDataSource" DataObjectTypeName="bulksms.MsgMain" UpdateMethod="Update" TypeName="bulksms.MsgMain" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="MsgMain_MsgidList" DataTextField="Msgid" DataValueField="Msgid" SelectedValue='<%# Bind("Msgid") %>' DataSourceID="MsgMain_MsgidDataSource" >
</asp:DropDownList>
</InsertItemTemplate>
</asp:TemplateField>
<asp:BoundField DataField="Hide" HeaderText="Hide"></asp:BoundField>
<asp:BoundField ReadOnly="True" DataField="MsgDetailId" InsertVisible="False" HeaderText="MsgDetailId"></asp:BoundField>
<asp:CommandField CancelText="Clear Values" ShowInsertButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetMsgDetail" ID="MsgDetailsDataSource" DataObjectTypeName="bulksms.MsgDetail" UpdateMethod="Update" TypeName="bulksms.MsgDetail" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="MsgDetailId" QueryStringField="MsgDetailId"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>
