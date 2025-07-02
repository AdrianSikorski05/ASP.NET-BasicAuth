using AplikacjaAndroid.Models.Dto.Comment;

namespace AplikacjaAndroid
{
    public interface ICommentService
    {
        Task<ResponseResult<PagedResult<CommentBook>>> GetComments(CommentFilter commentFilter);
        Task<ResponseResult<CommentBook?>> AddCommentAsync(CreateCommentBookDto comment);
        Task<ResponseResult<CommentBook?>> UpdateCommentAsync(UpdateCommentBookDto comment);
        Task<ResponseResult<bool>> DeleteCommentAsync(int id);
    }
}
