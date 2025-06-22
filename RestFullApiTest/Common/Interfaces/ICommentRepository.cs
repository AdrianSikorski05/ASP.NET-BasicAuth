namespace RestFullApiTest
{
    public interface ICommentRepository
    {
        /// <summary>
        /// Get all comments with pagination and filtering.
        /// </summary>
        /// <param name="commentFilter">Filter parameters for comments.</param>
        /// <returns>Paginated list of comments.</returns>
        Task<PagedResult<CommentBook>> GetCommentsAsync(CommentFilter commentFilter);

        /// <summary>
        /// Add a new comment to a book.
        /// </summary>
        /// <param name="comment">Comment to be added.</param>
        /// <returns>Added comment.</returns>
        Task<CommentBook> AddCommentAsync(CreateCommentBookDto comment);
        
        /// <summary>
        /// Update an existing comment.
        /// </summary>
        /// <param name="comment">Comment to be updated.</param>
        /// <returns>Updated comment.</returns>
        Task<CommentBook> UpdateCommentAsync(UpdateCommentBookDto comment);
        
        /// <summary>
        /// Delete a comment by its ID.
        /// </summary>
        /// <param name="id">ID of the comment to be deleted.</param>
        Task DeleteCommentAsync(int id);
    }
}
