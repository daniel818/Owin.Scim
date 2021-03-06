﻿namespace Owin.Scim.Repository.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Extensions;

    using Model.Groups;
    using Model.Users;

    using Owin.Scim.Configuration;
    using Querying;

    /// <summary>
    /// This could have been implemented by InMemoryUserRepository
    /// </summary>
    public class InMemoryGroupRepository : IGroupRepository
    {
        private readonly IDictionary<string, ScimGroup> _Groups;
        private readonly ScimServerConfiguration _scimServerConfiguration;

        public InMemoryGroupRepository(ScimServerConfiguration scimServerConfiguration)
        {
            _Groups = new Dictionary<string, ScimGroup>();
            _scimServerConfiguration = scimServerConfiguration;
        }

        public async Task<ScimGroup> CreateGroup(ScimGroup group)
        {
            group.Id = Guid.NewGuid().ToString("N");

            var createdDate = DateTime.UtcNow;
            group.Meta.Created = createdDate;
            group.Meta.LastModified = createdDate;

            _Groups.Add(group.Id, group);

            return group;
        }

        public async Task<ScimGroup> GetGroup(string groupId)
        {
            if (_Groups.ContainsKey(groupId))
                return _Groups[groupId].Copy();

            return null;
        }

        public async Task<ScimGroup> UpdateGroup(ScimGroup group)
        {
            if (!_Groups.ContainsKey(group.Id))
                return null;

            group.Meta.LastModified = DateTime.UtcNow;
            _Groups[group.Id] = group;

            return group;
        }

        public async Task DeleteGroup(string groupId)
        {
            if (!_Groups.ContainsKey(groupId))
                return;

            var groupRecord = _Groups[groupId];
            _Groups.Remove(groupId);
        }

        public async Task<IEnumerable<ScimGroup>> QueryGroups(ScimQueryOptions options)
        {
            var groups = _Groups.Values.AsEnumerable();
            if (options.Filter != null)
                groups = groups.Where(options.Filter.ToPredicate<ScimGroup>(_scimServerConfiguration)).ToList();

            // TODO: (DG) sorting
            if (options.SortBy != null)
            {
            }

            if (options.StartIndex > 1)
                groups = groups.Skip(options.StartIndex);

            if (options.Count > 0)
                groups = groups.Take(options.Count);

            return groups;
        }

        public async Task<IEnumerable<UserGroup>> GetGroupsUserBelongsTo(string userId)
        {
            // TODO: (CY) need to add indirect groups too
            return _Groups
                .Values
                .Where(g => g.Members != null && g.Members.Any(m => m.Value.Equals(userId)))
                .Select(group => new UserGroup
                {
                    Value = group.Id,
                    Display = group.DisplayName,
                    Type = "direct"
                });
        }

        public Task<bool> GroupExists(string groupId)
        {
            return Task.FromResult(_Groups.ContainsKey(groupId));
        }
    }
}