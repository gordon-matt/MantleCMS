using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.JQQueryBuilder
{
    public interface IJQQueryBuilderController
    {
        IActionResult GetQueryConfig();

        Task<IActionResult> ExecuteQuery([FromBody] JQQueryBuilderRawQuery query);

        Task<IActionResult> LoadQuery(Guid key);

        Task<IActionResult> SaveQuery([FromBody] JQQueryBuilderSavedQuery query);
    }

    public class JQQueryBuilderRawQuery
    {
        public string Query { get; set; }

        public int Skip { get; set; }

        public short Take { get; set; }
    }

    public class JQQueryBuilderSavedQuery
    {
        public string Name { get; set; }

        public string Query { get; set; }
    }
}