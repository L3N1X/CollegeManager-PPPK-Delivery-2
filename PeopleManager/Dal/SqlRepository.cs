using PeopleManager.Models;
using PeopleManager.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PeopleManager.Dal
{
    internal class SqlRepository : IRepository
    {
        private Student ReadStudent(SqlDataReader dr)
        => new Student
        {
            Id = (int)dr[nameof(Student.Id)],
            Age = (int)dr[nameof(Student.Age)],
            FirstName = dr[nameof(Student.FirstName)].ToString(),
            LastName = dr[nameof(Student.LastName)].ToString(),
            Email = dr[nameof(Student.Email)].ToString(),
            Picture = dr[nameof(Student.Picture)] != DBNull.Value ? ImageUtils.ByteArrayFromSqlDataReader(dr, 5) : null,
        };
        private Lecturer ReadLecturer(SqlDataReader dr)
        => new Lecturer
        {
            Id = (int)dr[nameof(Lecturer.Id)],
            Age = (int)dr[nameof(Lecturer.Age)],
            FirstName = dr[nameof(Lecturer.FirstName)].ToString(),
            LastName = dr[nameof(Lecturer.LastName)].ToString(),
            Email = dr[nameof(Lecturer.Email)].ToString(),
            Picture = dr[nameof(Lecturer.Picture)] != DBNull.Value ? ImageUtils.ByteArrayFromSqlDataReader(dr, 5) : null,
        };
        private Subject ReadSubject(SqlDataReader dr)
        => new Subject
        {
            Id = (int)dr[nameof(Subject.Id)],
            Name = dr[nameof(Subject.Name)].ToString(),
            LecturerId = (int)dr[nameof(Subject.LecturerId)],
        };
        private StudentSubject ReadStudentSubject(SqlDataReader dr)
        => new StudentSubject
        {
            Id = (int)dr[nameof(StudentSubject.Id)],
            StudentId = (int)dr[nameof(StudentSubject.StudentId)],
            SubjectId = (int)dr[nameof(StudentSubject.SubjectId)],
        };
        private static string cs = ConfigurationManager.ConnectionStrings["cs"].ConnectionString;
        public void AddStudent(Student student)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = MethodBase.GetCurrentMethod().Name;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(nameof(Student.FirstName), student.FirstName);
                    cmd.Parameters.AddWithValue(nameof(Student.LastName), student.LastName);
                    cmd.Parameters.AddWithValue(nameof(Student.Age), student.Age);
                    cmd.Parameters.AddWithValue(nameof(Student.Email), student.Email);

                    if(student.Picture != null)
                    {
                        cmd.Parameters.Add(new SqlParameter(nameof(Student.Picture), System.Data.SqlDbType.Binary, student.Picture.Length)
                        {
                            Value = student.Picture
                        });
                    }
                    else
                    {
                        cmd.Parameters.Add(new SqlParameter(nameof(Student.Picture), System.Data.SqlDbType.Binary, 0)
                        {
                            Value = new byte[0]
                        });
                    }

                    SqlParameter id = new SqlParameter(
                        nameof(Student.Id),
                        System.Data.SqlDbType.Int)
                    {
                        Direction = System.Data.ParameterDirection.Output
                    };
                    cmd.Parameters.Add(id);
                    cmd.ExecuteNonQuery();
                    student.Id = (int)id.Value;
                }
                foreach (Subject subject in student.Subjects)
                {
                    this.AddStudentSubject(new StudentSubject()
                    {
                        StudentId = student.Id,
                        SubjectId = subject.Id,
                    });
                }
            }
        }

        public void DeleteStudent(Student student)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = MethodBase.GetCurrentMethod().Name;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(nameof(Student.Id), student.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public ICollection<Student> GetStudents()
        {
            IList<Student> list = new List<Student>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = MethodBase.GetCurrentMethod().Name;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Student student = ReadStudent(dr);

                            var studentSubjects = this.GetStudentSubjects().Where(sb => sb.StudentId == student.Id);

                            student.Subjects = studentSubjects.Select(sb => this.GetSubject(sb.SubjectId)).ToList();

                            list.Add(student);
                        }
                    }

                }
            }
            return list;
        }

        public Student GetStudent(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = MethodBase.GetCurrentMethod().Name;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(nameof(Student.Id), id);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return ReadStudent(dr);
                        }
                    }

                }
            }
            throw new Exception("Wrong id");
        }

        public void UpdateStudent(Student student)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = MethodBase.GetCurrentMethod().Name;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(nameof(Student.FirstName), student.FirstName);
                    cmd.Parameters.AddWithValue(nameof(Student.LastName), student.LastName);
                    cmd.Parameters.AddWithValue(nameof(Student.Age), student.Age);
                    cmd.Parameters.AddWithValue(nameof(Student.Email), student.Email);
                    cmd.Parameters.AddWithValue(nameof(Student.Id), student.Id);

                    cmd.Parameters.Add(new SqlParameter(
                        nameof(Student.Picture),
                        System.Data.SqlDbType.Binary,
                        student.Picture.Length)
                    {
                        Value = student.Picture
                    });
                    cmd.ExecuteNonQuery();
                }
                var studentSubjects = GetStudentSubjects().Where(sb => sb.StudentId == student.Id);
                var subjectIds = studentSubjects.Select(sb => sb.SubjectId);

                IList<int> subjectIdsToAdd = new List<int>();
                IList<int> subjectIdsToRemove = new List<int>();

                student.Subjects.ToList().ForEach(s => 
                {
                    if(!subjectIds.Contains(s.Id))
                        subjectIdsToAdd.Add(s.Id);
                });

                var currentStudentSubjectIds = studentSubjects.Select(sb => sb.Id);

                subjectIds.ToList().ForEach(sid =>
                {
                    if(!currentStudentSubjectIds.Contains(sid))
                    {
                        subjectIdsToRemove.Add(sid);
                    }
                });

                foreach (var sid in subjectIdsToRemove)
                {
                    this.DeleteStudentSubject(sid);
                }
                foreach (var sid in subjectIdsToAdd)
                {
                    this.AddStudentSubject(new StudentSubject { StudentId = student.Id, SubjectId = sid });
                }
            }
        }

        public void AddLecturer(Lecturer lecturer)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = MethodBase.GetCurrentMethod().Name;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(nameof(Lecturer.FirstName), lecturer.FirstName);
                    cmd.Parameters.AddWithValue(nameof(Lecturer.LastName), lecturer.LastName);
                    cmd.Parameters.AddWithValue(nameof(Lecturer.Age), lecturer.Age);
                    cmd.Parameters.AddWithValue(nameof(Lecturer.Email), lecturer.Email);

                    if (lecturer.Picture != null)
                    {
                        cmd.Parameters.Add(new SqlParameter(nameof(Lecturer.Picture), System.Data.SqlDbType.Binary, lecturer.Picture.Length)
                        {
                            Value = lecturer.Picture
                        });
                    }
                    else
                    {
                        cmd.Parameters.Add(new SqlParameter(nameof(Lecturer.Picture), System.Data.SqlDbType.Binary, 0)
                        {
                            Value = new byte[0]
                        });
                    }
                    SqlParameter id = new SqlParameter(
                        nameof(Lecturer.Id),
                        System.Data.SqlDbType.Int)
                    {
                        Direction = System.Data.ParameterDirection.Output
                    };
                    cmd.Parameters.Add(id);
                    cmd.ExecuteNonQuery();
                    lecturer.Id = (int)id.Value;
                }
                foreach (var subject in lecturer.Subjects)
                {
                    subject.LecturerId = lecturer.Id;
                    this.UpdateSubject(subject);
                }
            }
        }

        public void UpdateLecturer(Lecturer lecturer)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = MethodBase.GetCurrentMethod().Name;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(nameof(Lecturer.FirstName), lecturer.FirstName);
                    cmd.Parameters.AddWithValue(nameof(Lecturer.LastName), lecturer.LastName);
                    cmd.Parameters.AddWithValue(nameof(Lecturer.Age), lecturer.Age);
                    cmd.Parameters.AddWithValue(nameof(Lecturer.Email), lecturer.Email);
                    cmd.Parameters.AddWithValue(nameof(Lecturer.Id), lecturer.Id);

                    cmd.Parameters.Add(new SqlParameter(
                        nameof(Lecturer.Picture),
                        System.Data.SqlDbType.Binary,
                        lecturer.Picture.Length)
                    {
                        Value = lecturer.Picture
                    });
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteLecturer(Lecturer lecturer)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = MethodBase.GetCurrentMethod().Name;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(nameof(Lecturer.Id), lecturer.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public ICollection<Lecturer> GetLecturers()
        {
            IList<Lecturer> list = new List<Lecturer>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = MethodBase.GetCurrentMethod().Name;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Lecturer lecturer = ReadLecturer(dr);

                            var subjects = this.GetSubjects().Where(subject => subject.LecturerId == lecturer.Id);
                            lecturer.Subjects = subjects.ToList();

                            list.Add(lecturer);
                        }
                    }

                }
            }
            return list;
        }

        public Lecturer GetLecturer(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = MethodBase.GetCurrentMethod().Name;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(nameof(Lecturer.Id), id);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return ReadLecturer(dr);
                        }
                    }

                }
            }
            throw new Exception("Wrong id");
        }

        public void AddSubject(Subject subject)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = MethodBase.GetCurrentMethod().Name;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(nameof(Subject.Name), subject.Name);
                    cmd.Parameters.AddWithValue(nameof(Subject.LecturerId), subject.LecturerId);
                    SqlParameter id = new SqlParameter(
                        nameof(Subject.Id),
                        System.Data.SqlDbType.Int)
                    {
                        Direction = System.Data.ParameterDirection.Output
                    };
                    cmd.Parameters.Add(id);
                    cmd.ExecuteNonQuery();
                    subject.Id = (int)id.Value;
                }
            }
        }

        public void UpdateSubject(Subject subject)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = MethodBase.GetCurrentMethod().Name;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(nameof(Subject.Id), subject.Id);
                    cmd.Parameters.AddWithValue(nameof(Subject.Name), subject.Name);
                    cmd.Parameters.AddWithValue(nameof(Subject.LecturerId), subject.LecturerId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteSubject(Subject subject)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = MethodBase.GetCurrentMethod().Name;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(nameof(Subject.Id), subject.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public ICollection<Subject> GetSubjects()
        {
            IList<Subject> list = new List<Subject>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = MethodBase.GetCurrentMethod().Name;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Subject subject = ReadSubject(dr);
                            subject.Lecturer = this.GetLecturer(subject.LecturerId);

                            var studentSubjects = this.GetStudentSubjects().Where(sb => sb.SubjectId == subject.Id);

                            subject.Students = studentSubjects.Select(sb => this.GetStudent(sb.StudentId)).ToList();
                            list.Add(subject);
                        }
                    }

                }
            }
            return list;
        }

        public Subject GetSubject(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = MethodBase.GetCurrentMethod().Name;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(nameof(Subject.Id), id);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return ReadSubject(dr);
                        }
                    }
                }
            }
            throw new Exception("Wrong id");
        }

        public ICollection<StudentSubject> GetStudentSubjects()
        {
            IList<StudentSubject> list = new List<StudentSubject>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = MethodBase.GetCurrentMethod().Name;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(ReadStudentSubject(dr));
                        }
                    }

                }
            }
            return list;
        }

        public void AddStudentSubject(StudentSubject studentSubject)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = MethodBase.GetCurrentMethod().Name;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(nameof(StudentSubject.StudentId), studentSubject.StudentId);
                    cmd.Parameters.AddWithValue(nameof(StudentSubject.SubjectId), studentSubject.SubjectId);

                    SqlParameter id = new SqlParameter(
                        nameof(StudentSubject.Id),
                        System.Data.SqlDbType.Int)
                    {
                        Direction = System.Data.ParameterDirection.Output
                    };
                    cmd.Parameters.Add(id);
                    cmd.ExecuteNonQuery();
                    studentSubject.Id = (int)id.Value;
                }
            }
        }

        public void DeleteStudentSubject(int sid)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = MethodBase.GetCurrentMethod().Name;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("Id", sid);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
