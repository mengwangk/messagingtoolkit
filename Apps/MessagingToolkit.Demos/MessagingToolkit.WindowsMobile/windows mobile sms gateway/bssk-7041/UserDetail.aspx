<%@ Page  MasterPageFile="~/MasterPage.master" Title="User Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="mainContentPlaceholder" Runat="Server">
   <div class="pagecontent">
        <h1>User Detail</h1>      

<asp:DetailsView runat="server" DataSourceID="UsersDataSource" EmptyDataText="The requested record was not found." ID="UsersDetailsView" AutoGenerateRows="False" DataKeyNames="Username"><Fields>
<asp:BoundField ReadOnly="True" DataField="Username" InsertVisible="False" HeaderText="Username"></asp:BoundField>
<asp:TemplateField SortExpression="ProorityGroup" HeaderText="ProorityGroup"><EditItemTemplate>
<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" SelectMethod="GetAllPriorityGroups" ID="PriorityGroup_ProorityGroupDataSource" DataObjectTypeName="bulksms.PriorityGroup" UpdateMethod="Update" TypeName="bulksms.PriorityGroup" OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>

<asp:DropDownList runat="server" ID="PriorityGroup_ProorityGroupList" DataTextField="PriorityGroupId" DataValueField="PriorityGroupId" SelectedValue='<%# Bind("ProorityGroup") %>' DataSourceID="PriorityGroup_ProorityGroupDataSource" >
</asp:DropDownList>
</EditItemTemplate>
<ItemTemplate>
<asp:HyperLink ID="ProorityGroupLink" runat="server" Text='<%# Eval("ProorityGroup") %>' NavigateUrl='<%# Eval("ProorityGroup", "~/PriorityGroupDetail.aspx?PriorityGroupId={0}")%>' />
</ItemTemplate>
</asp:TemplateField>
<asp:BoundField DataField="Userid" InsertVisible="False" HeaderText="Userid"></asp:BoundField>
<asp:BoundField DataField="Name" InsertVisible="False" HeaderText="Name"></asp:BoundField>
<asp:BoundField DataField="Country" InsertVisible="False" HeaderText="Country"></asp:BoundField>
<asp:BoundField DataField="City" InsertVisible="False" HeaderText="City"></asp:BoundField>
<asp:BoundField DataField="Sex" InsertVisible="False" HeaderText="Sex"></asp:BoundField>
<asp:BoundField DataField="Birthday" InsertVisible="False" HeaderText="Birthday"></asp:BoundField>
<asp:BoundField DataField="Tel" InsertVisible="False" HeaderText="Tel"></asp:BoundField>
<asp:BoundField DataField="Mobileno" InsertVisible="False" HeaderText="Mobileno"></asp:BoundField>
<asp:BoundField DataField="Mobileno2" InsertVisible="False" HeaderText="Mobileno2"></asp:BoundField>
<asp:BoundField DataField="Fax" InsertVisible="False" HeaderText="Fax"></asp:BoundField>
<asp:BoundField DataField="Email" InsertVisible="False" HeaderText="Email"></asp:BoundField>
<asp:BoundField DataField="Website" InsertVisible="False" HeaderText="Website"></asp:BoundField>
<asp:BoundField DataField="Regcode" InsertVisible="False" HeaderText="Regcode"></asp:BoundField>
<asp:BoundField DataField="Active" InsertVisible="False" HeaderText="Active"></asp:BoundField>
<asp:BoundField DataField="Points" InsertVisible="False" HeaderText="Points"></asp:BoundField>
<asp:BoundField DataField="Regdate" InsertVisible="False" HeaderText="Regdate"></asp:BoundField>
<asp:BoundField DataField="Passcode" InsertVisible="False" HeaderText="Passcode"></asp:BoundField>
<asp:BoundField DataField="Lastvisit" InsertVisible="False" HeaderText="Lastvisit"></asp:BoundField>
<asp:BoundField DataField="Expirydate" InsertVisible="False" HeaderText="Expirydate"></asp:BoundField>
<asp:BoundField DataField="Company" InsertVisible="False" HeaderText="Company"></asp:BoundField>
<asp:BoundField DataField="Timezone" InsertVisible="False" HeaderText="Timezone"></asp:BoundField>
<asp:HyperLinkField Text="View GrpMains" DataNavigateUrlFormatString="~/GrpMains.aspx?Table=User_GrpMains&amp;GrpMains_Username={0}" DataNavigateUrlFields="Username,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View MsgHistories" DataNavigateUrlFormatString="~/MsgHistories.aspx?Table=User_MsgHistories&amp;MsgHistories_Username={0}" DataNavigateUrlFields="Username,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View MsgMains" DataNavigateUrlFormatString="~/MsgMains.aspx?Table=User_MsgMains&amp;MsgMains_Username={0}" DataNavigateUrlFields="Username,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View MsgOutboxes" DataNavigateUrlFormatString="~/MsgOutboxes.aspx?Table=User_MsgOutboxes&amp;MsgOutboxes_Username={0}" DataNavigateUrlFields="Username,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View PointHistories" DataNavigateUrlFormatString="~/PointHistories.aspx?Table=User_PointHistories&amp;PointHistories_Username={0}" DataNavigateUrlFields="Username,"></asp:HyperLinkField>
<asp:HyperLinkField Text="View SenderIds" DataNavigateUrlFormatString="~/SenderIds.aspx?Table=User_SenderIds&amp;SenderIds_Username={0}" DataNavigateUrlFields="Username,"></asp:HyperLinkField>
<asp:CommandField ShowDeleteButton="True" ShowEditButton="True"></asp:CommandField>
</Fields>
</asp:DetailsView>

<asp:ObjectDataSource runat="server" DeleteMethod="Delete" EnableCaching="True" ConflictDetection="CompareAllValues" InsertMethod="Insert" SelectMethod="GetUser" ID="UsersDataSource" DataObjectTypeName="bulksms.User" UpdateMethod="Update" TypeName="bulksms.User" OldValuesParameterFormatString="original_{0}"><SelectParameters>
<asp:QueryStringParameter ConvertEmptyStringToNull="False" Name="Username" QueryStringField="Username"></asp:QueryStringParameter>
</SelectParameters>
</asp:ObjectDataSource>

    </div>
</asp:Content>
