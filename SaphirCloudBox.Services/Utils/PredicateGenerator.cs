using LinqKit;
using SaphirCloudBox.Models;
using SaphirCloudBox.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaphirCloudBox.Services.Utils
{
    public static class PredicateGenerator
    {
        public static IQueryable<FileStorage> GetForFileStorage(this IQueryable<FileStorage> query, AdvancedSearchDto advancedSearch)
        {
            var predicate = PredicateBuilder.New<FileStorage>(true);

            if (IsNotEmptySearch(advancedSearch))
            {
                predicate.GetByClients(advancedSearch.ClientIds);
                predicate.GetByDepartments(advancedSearch.DepartmentIds);
                predicate.GetByUsers(advancedSearch.UserIds);
                predicate.GetByUserGroups(advancedSearch.UserGroupIds);
                predicate.GetByDate(advancedSearch.StartDate, advancedSearch.EndDate);
                predicate.GetBySearchString(advancedSearch.SearchString);
            }
            else
            {
                predicate.And(x => !x.IsActive);
            }
            

            return query.Where(predicate);
        }

        private static bool IsNotEmptySearch(AdvancedSearchDto advancedSearch)
        {
            return advancedSearch.ClientIds.Count() > 0 
                || advancedSearch.DepartmentIds.Count() > 0
                || advancedSearch.UserIds.Count() > 0
                || advancedSearch.UserGroupIds.Count() > 0
                || advancedSearch.StartDate.HasValue
                || advancedSearch.EndDate.HasValue
                || !String.IsNullOrEmpty(advancedSearch.SearchString);
        }

        private static void GetByClients(this ExpressionStarter<FileStorage> predicate, IEnumerable<int> clientIds)
        {
            if (clientIds.Count() > 0)
            {
                predicate.And(x => x.ClientId.HasValue && clientIds.Contains(x.ClientId.Value)
                                                || x.OwnerId.HasValue && clientIds.Contains(x.Owner.ClientId));
            }
        }

        private static void GetByDepartments(this ExpressionStarter<FileStorage> predicate, IEnumerable<int> departmentIds)
        {
            if (departmentIds.Count() > 0)
            {
                predicate.And(x => x.ClientId.HasValue && x.Client.Departments.Any(y => departmentIds.Contains(y.Id))
                                                || x.OwnerId.HasValue && x.Owner.DepartmentId.HasValue && departmentIds.Contains(x.Owner.DepartmentId.Value));
            } 
        }

        private static void GetByUsers(this ExpressionStarter<FileStorage> predicate, IEnumerable<int> userIds)
        {
            if (userIds.Count() > 0)
            {
                predicate.And(x => x.OwnerId.HasValue && userIds.Contains(x.OwnerId.Value)
                                                || x.ClientId.HasValue && x.Client.Users.Any(y => userIds.Contains(x.Id)));
            }
        }

        private static void GetByUserGroups(this ExpressionStarter<FileStorage> predicate, IEnumerable<int> userGroupIds)
        {
            if (userGroupIds.Count() > 0)
            {
                predicate.And(x => x.OwnerId.HasValue && x.Owner.UserInGroups.Any(y => userGroupIds.Contains(y.GroupId)));
            }
        }

        private static void GetByDate(this ExpressionStarter<FileStorage> predicate, DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                predicate.And(x => startDate <= x.CreateDate && x.CreateDate <= endDate);
            }
            else if (startDate.HasValue && !endDate.HasValue)
            {
                predicate.And(x => startDate <= x.CreateDate);
            }
            else if (!startDate.HasValue && endDate.HasValue)
            {
                predicate.And(x => x.CreateDate <= endDate);
            }
        }

        private static void GetBySearchString(this ExpressionStarter<FileStorage> predicate, string searchString)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                var strings = searchString.Split(".");
                predicate.And(x => searchString.Contains(String.Join("", x.Name, x.Files.FirstOrDefault(y => y.IsActive) != null ? x.Files.FirstOrDefault(y => y.IsActive).Extension : String.Empty)));
            }
        }
    }
}
