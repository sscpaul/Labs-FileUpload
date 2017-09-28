using NFine.Code;
using NFine.Domain.Entity.FileManage;
using NFine.Domain.Entity.SystemManage;
using NFine.Application.SystemManage;
using NFine.Domain.IRepository.FileManage;
using NFine.Repository.FileManage;
using NFine.Domain.IRepository.SystemManage;
using NFine.Repository.SystemManage;
using System;
using System.Collections.Generic;

namespace NFine.Application.FileManage
{
    public class AttachmentApp
    {
        private IAttachmentRepository service = new AttachmentRepository();

        // 获取用逗号分隔的所有子部门ID
        private string GetAllChild(List<OrganizeEntity> orgList, string departID)
        {
            string childIDs = "";

            orgList.FindAll(t => t.F_ParentId.Equals(departID, StringComparison.CurrentCultureIgnoreCase)).ForEach(t =>
            {
                childIDs += (childIDs.IsEmpty() ? "" : ",") + "'" + t.F_Id + "'";
                string tmpIDs = GetAllChild(orgList, t.F_Id);
                if (!tmpIDs.IsEmpty())
                    childIDs += "," + tmpIDs;
            });

            return childIDs;
        }

        public List<AttachmentEntity> GetList(Pagination pagination, string keyword)
        {
            // 获取当前登录用户ID和部门ID
            string userID = OperatorProvider.Provider.GetCurrent().UserId;
            string departID = OperatorProvider.Provider.GetCurrent().DepartmentId;
            // 获取当前用户实体对象
            UserApp userApp = new UserApp();
            UserEntity user = userApp.GetList().Find(t => t.F_Id == userID);
            // 查找当前用户所在部门及下级部门
            string childIDs = GetAllChild((new OrganizeApp()).GetList(), departID);
            string departandChildIDs = "'" + departID + "'" + (childIDs.IsEmpty() ? "" : "," + childIDs);
            departandChildIDs = departandChildIDs.ToLower();

            var expression = ExtLinq.True<AttachmentEntity>();
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.F_Name.Contains(keyword));
            }
            if (user != null)
            {
                switch (user.F_AttachmentLevel)
                {
                    case 1:
                        // 仅本人
                        expression = expression.And(t => t.F_CreatorUserId.Equals(userID, StringComparison.CurrentCultureIgnoreCase));
                        break;
                    case 2:
                        // 仅本部门
                        {
                            var userList = userApp.GetList().FindAll(t => t.F_DepartmentId.Equals(departID, StringComparison.CurrentCultureIgnoreCase));
                            string userIDs = ""; userList.ForEach(t => userIDs += (userIDs.IsEmpty() ? "'" : ",'") + t.F_Id.ToLower() + "'");
                            expression = expression.And(t => userIDs.Contains(t.F_CreatorUserId));
                        }
                        break;
                    case 3:
                        // 本部门及下级
                        {
                            var userList = userApp.GetList().FindAll(t => departandChildIDs.Contains(t.F_DepartmentId.ToLower()));
                            string userIDs = ""; userList.ForEach(t => userIDs += (userIDs.IsEmpty() ? "'" : ",'") + t.F_Id.ToLower() + "'");
                            expression = expression.And(t => userIDs.Contains(t.F_CreatorUserId));
                        }
                        break;
                    case 9:
                        // 所有
                        break;
                    default:
                        break;
                }
            }
            else
            {
                expression = expression.And(t => false);
            }

            return service.FindList(expression, pagination);
        }
        public AttachmentEntity GetForm(string keyValue)
        {
            return service.FindEntity(keyValue);
        }
        public void DeleteForm(string keyValue)
        {
            service.DeleteForm(keyValue);
        }
        public void SubmitForm(AttachmentEntity attachmentEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                attachmentEntity.Modify(keyValue);
            }
            else
            {
                attachmentEntity.Create();
            }
            service.SubmitForm(attachmentEntity, keyValue);
        }
    }
}
