namespace RefitDemo.API
{
    using System.Threading.Tasks;

    using Refit;

    using RefitDemo.API.Models;

    /// <summary>
    /// The UserService interface.
    /// SOURCE DOC FOR MORE INFO: https://reactiveui.github.io/refit/
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// The get async.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Get("/api/users")]
        public Task<Root> GetAsync([Query] int page);
    }
}
