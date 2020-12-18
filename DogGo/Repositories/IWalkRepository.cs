using DogGo.Models;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public interface IWalkRepository
    {
        List<Walk> GetWalksByWalkerId(int id);
        List<Walk> GetWalksByDogId(int id);
        List<Walk> GetUpcomingWalksByOwnerId(int id);
        void AddWalk(Walk walk);
        void UpdateWalk(Walk walk);
    }
}