<%@ Page  MasterPageFile="~/MasterPage.master" Title="MsgDetails"%>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>MsgDetails</h1>
<asp:GridView runat="server" DataSourceID="MsgDetailsDataSource" ID="MsgDetailsGridView" AllowSorting="True" EmptyDataText="No records were returned." AutoGenerateColumns="False" AllowPaging="True" DataKeyNames="MsgDetailId"><Columns>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
<asp:BoundField DataField="Mobile" SortExpression="Mobile" HeaderText="Mobile"></asp:BoundField>
<asp:TemplateField SortExpression="Groupid" HeaderText="Groupid"><EditItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllGrpMains" ID="GrpMain_GroupidDataSource" DataObjectTypeName="bulksms.GrpMain" UpdateMethod="Update" TypeName="bulksms.GrpMain" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="GrpMain_GroupidList" DataTextField="Groupid" DataValueField="Groupid" SelectedValue='<%# Bind("Groupid") %>' DataSourceID="GrpMain_GroupidDataSource" >
</asp:DropDownList>
</EditItemTemplate>
<ItemTemplate>
<asp:HyperLink ID="GroupidLink" runat="server" Text='<%# Eval("Groupid") %>' NavigateUrl='<%# Eval("Groupid", "~/GrpMainDetail.aspx?Groupid={0}")%>' />
</ItemTemplate>
</asp:TemplateField>
<asp:TemplateField SortExpression="Msgid" HeaderText="Msgid"><EditItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllMsgMains" ID="MsgMain_MsgidDataSource" DataObjectTypeName="bulksms.MsgMain" UpdateMethod="Update" TypeName="bulksms.MsgMain" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="MsgMain_MsgidList" DataTextField="Msgid" DataValueField="Msgid" SelectedValue='<%# Bind("Msgid") %>' DataSourceID="MsgMain_MsgidDataSource" >
</asp:DropDownList>
</EditItemTemplate>
<ItemTemplate>
<asp:HyperLink ID="MsgidLink" runat="server" Text='<%# Eval("Msgid") %>' NavigateUrl='<%# Eval("Msgid", "~/MsgMainDetail.aspx?Msgid={0}")%>' />
</ItemTemplate>
</asp:TemplateField>
<asp:BoundField DataField="Hide" SortExpression="Hide" HeaderText="Hide"></asp:BoundField>
<asp:BoundField ReadOnly="True" DataField="MsgDetailId" InsertVisible="False" SortExpression="MsgDetailId" HeaderText="MsgDetailId"></asp:BoundField>
<asp:HyperLinkField Text="View Details" DataNavigateUrlFormatString="~/MsgDetailDetail.aspx?MsgDetailId={0}" DataNavigateUrlFields="MsgDetailId"></asp:HyperLinkField>
</Columns>
</asp:GridView>

<asp:ObjectDataSource runat="server" SelectCountMethod="GetMsgDetailsCount" DeleteMethod="Delete" ConflictDetection="CompareAllValues" SortParameterName="sortExpression" SelectMethod="GetMsgDetails" ID="MsgDetailsDataSource" DataObjectTypeName="bulksms.MsgDetail" EnablePaging="True" UpdateMethod="Update" TypeName="bulksms.MsgDetail" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter Name="tableName" QueryStringField="Table"></asp:QueryStringParameter>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="MsgDetails_Msgid" QueryStringField="MsgDetails_Msgid"></asp:QueryStringParameter>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="MsgDetails_Groupid" QueryStringField="MsgDetails_Groupid"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

        <div class="link">
        <asp:HyperLink runat="server" CssClass="button" ID="NewRecordLink" NavigateUrl="~/NewMsgDetail.aspx">Create New MsgDetail</asp:HyperLink>

        </div>    
    </div>
</asp:Content>
