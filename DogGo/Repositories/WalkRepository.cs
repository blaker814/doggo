﻿using DogGo.Models;
using DogGo.Repositories.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogGo.Repositories
{
    public class WalkRepository : IWalkRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public WalkRepository(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection => new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        public Walk GetWalkById(int walkId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT w.Id, Date, Duration, DogId, WalkerId, WalkStatusId, Name
                                        FROM Walks w
                                        JOIN Walker ON Walker.Id = WalkerId
                                        WHERE w.Id = @id";
                    cmd.Parameters.AddWithValue("@id", walkId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if(reader.Read())
                    {
                        Walk walk = new Walk
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                            DogId = reader.GetInt32(reader.GetOrdinal("DogId")),
                            WalkerId = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                            Walker = new Walker
                            {
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            },
                            WalkStatusId = reader.GetInt32(reader.GetOrdinal("WalkStatusId"))
                        };

                        reader.Close();
                        return walk;
                    }
                    else
                    {
                        reader.Close();
                        return null;
                    }
                }
            }
        }

        public List<Walk> GetWalksByWalkerId(int walkerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT w.Id, Date, Duration, WalkerId, DogId, WalkStatusId, Description, Name, OwnerId, ImageUrl
                                        FROM Walks w
                                        JOIN WalkStatus ws ON ws.Id = WalkStatusId
                                        JOIN Dog ON Dog.Id = DogId
                                        WHERE WalkerId = @id";
                    cmd.Parameters.AddWithValue("@id", walkerId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walk> walks = new List<Walk>();
                    while(reader.Read())
                    {
                        walks.Add(new Walk
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                            WalkerId = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                            DogId = reader.GetInt32(reader.GetOrdinal("DogId")),
                            Dog = new Dog
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DogId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                ImageUrl = ReaderUtils.GetNullableString(reader, "ImageUrl"),
                                OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId"))
                            },
                            WalkStatusId = reader.GetInt32(reader.GetOrdinal("WalkStatusId")),
                            WalkStatus = new WalkStatus
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("WalkStatusId")),
                                Description = reader.GetString(reader.GetOrdinal("Description"))
                            }
                        });
                    }
                    reader.Close();

                    return walks;
                }
            }
        }

        public List<Walk> GetWalksByDogId(int dogId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT w.Id, Date, Duration, WalkerId, DogId, WalkStatusId, Description, Walker.Name
                                        FROM Walks w
                                        JOIN Walker ON Walker.Id = WalkerId
                                        JOIN WalkStatus ws ON ws.Id = WalkStatusId
                                        WHERE DogId = @id";
                    cmd.Parameters.AddWithValue("@id", dogId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walk> walks = new List<Walk>();
                    while (reader.Read())
                    {
                        walks.Add(new Walk
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                            WalkerId = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                            Walker = new Walker
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            },
                            DogId = reader.GetInt32(reader.GetOrdinal("DogId")),
                            WalkStatusId = reader.GetInt32(reader.GetOrdinal("WalkStatusId")),
                            WalkStatus = new WalkStatus
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("WalkStatusId")),
                                Description = reader.GetString(reader.GetOrdinal("Description"))
                            }
                        });
                    }
                    reader.Close();

                    return walks;
                }
            }
        }

        public List<Walk> GetUpcomingWalksByOwnerId(int ownerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT w.Id, Date, Duration, WalkerId, DogId, WalkStatusId, Description
                                        FROM Walks w
                                        JOIN Dog d ON d.Id = DogId
                                        JOIN WalkStatus ws ON ws.Id = WalkStatusId
                                        WHERE OwnerId = @id AND WalkStatusId != 3";
                    cmd.Parameters.AddWithValue("@id", ownerId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walk> walks = new List<Walk>();
                    while (reader.Read())
                    {
                        walks.Add(new Walk
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                            WalkerId = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                            DogId = reader.GetInt32(reader.GetOrdinal("DogId")),
                            WalkStatusId = reader.GetInt32(reader.GetOrdinal("WalkStatusId")),
                            WalkStatus = new WalkStatus
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("WalkStatusId")),
                                Description = reader.GetString(reader.GetOrdinal("Description"))
                            }
                        });
                    }
                    reader.Close();

                    return walks;
                }
            }
        }

        public void AddWalk(Walk walk)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    INSERT INTO Walks (Date, Duration, WalkerId, DogId, WalkStatusId)
                    OUTPUT INSERTED.ID
                    VALUES (@date, @duration, @walkerId, @dogId, @walkStatusId);
                ";

                    cmd.Parameters.AddWithValue("@date", walk.Date);
                    cmd.Parameters.AddWithValue("@duration", walk.Duration);
                    cmd.Parameters.AddWithValue("@walkerId", walk.WalkerId);
                    cmd.Parameters.AddWithValue("@dogId", walk.DogId);
                    cmd.Parameters.AddWithValue("@walkStatusId", walk.WalkStatusId);

                    int id = (int)cmd.ExecuteScalar();

                    walk.Id = id;
                }
            }
        }

        public void UpdateWalk(Walk walk)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            UPDATE Walks
                            SET 
                                Date = @date,
                                Duration = @duration,
                                WalkerId = @walkerId,
                                DogId = @dogId,
                                WalkStatusId = @walkStatusId
                            WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@date", walk.Date);
                    cmd.Parameters.AddWithValue("@duration", walk.Duration);
                    cmd.Parameters.AddWithValue("@walkerId", walk.WalkerId);
                    cmd.Parameters.AddWithValue("@dogId", walk.DogId);
                    cmd.Parameters.AddWithValue("@walkStatusId", walk.WalkStatusId);
                    cmd.Parameters.AddWithValue("@id", walk.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
