using System;
using Company.ClassLibrary1;

namespace API.Interfaces;

public interface IUnitOfWork
{
  IMemberRepository MemberRepository { get; }
  IMessageRepository MessageRepository { get; }
  ILikesRepository LikesRepository { get; }
  IPhotoRepository PhotoRepository { get; }
  Task<bool> Complete(); // for saving changes of the repositories to the database

  bool HasChanges(); // to check the EF change tracker to see if there are any changes to save to the database
}
