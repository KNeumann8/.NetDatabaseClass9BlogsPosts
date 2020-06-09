using NLog;
using BlogsConsole.Models;
using System;
using System.Linq;
using System.Collections;

namespace BlogsConsole
{
    class MainClass
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static void Main(string[] args)
        {
            logger.Info("Program started");
            try
            {

                //program loop:
                bool masterLoopControl = true;
                do
                {
                    printUserChoiceMenu(); //print the menu
                    string input = Console.ReadLine(); //take the response
                    int userChoice = 4;
                    //force user to choose menu option that is correctly formatted
                    bool loopControl;
                    do
                    {
                        loopControl = false;
                        try
                        {
                            userChoice = int.Parse(input);
                            if (userChoice > 4 || userChoice <= 0)
                            {
                                FormatException e = new FormatException();
                                throw e;
                            }
                        }
                        catch (FormatException)
                        {
                            logger.Info("Improperly Formated Response: " + input);
                            loopControl = true;
                        }
                        catch (Exception e)
                        {
                            logger.Error("Unexpected Error at user choice: " + e.Message);
                            loopControl = true;
                        }
                    } while (loopControl);
                    //Execute the user choice:
                    switch (userChoice)
                    {
                        case 1:
                            displayBlogs();
                            break;
                        case 2:
                            createBlog();
                            break;
                        case 3:
                            createPost();
                            break;
                        case 4:
                            masterLoopControl = false;
                            break;
                    }
                } while (masterLoopControl);

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            logger.Info("Program ended");
        }



        private static void printUserChoiceMenu()
        {
            //Output Menu of options
            Console.WriteLine("Welcome to the Blog Console!");
            Console.WriteLine("1: View Blog Listings");
            Console.WriteLine("2: Create New Blog");
            Console.WriteLine("3: Add New Post to Blog");
            Console.WriteLine("4: Exit");
        }

        private static void createBlog()
        {
            // Create and save a new Blog
            Console.Write("Enter a name for a new Blog: ");
            var name = Console.ReadLine();

            var blog = new Blog { Name = name };

            var db = new BloggingContext();
            db.AddBlog(blog);
            logger.Info("Blog added - {name}", name);
        }

        private static void displayBlogs()
        {
            // Display all Blogs from the database
            var db = new BloggingContext();
            var query = db.Blogs.OrderBy(b => b.Name);

            Console.WriteLine("All blogs in the database:");
            foreach (var item in query)
            {
                Console.WriteLine(item.Name);
            }
        }

        private static void createPost()
        {
            //Create a new post

            var db = new BloggingContext();

            //Display all blogs for user to pick from
            Console.WriteLine("All blogs in the database:");      
            var queryOne = db.Blogs.OrderBy(b => b.Name);
            ArrayList blogIDs = new ArrayList();
            foreach (var item in queryOne)
            {
                Console.WriteLine(item.BlogId+": "+item.Name);
                blogIDs.Add(item.BlogId);
            }

            //Ask user to choose blog by id, force correct response
            bool loopControl;
            int userChoice = -1;
            do
            {
                loopControl = false;
                Console.WriteLine("What blog will you be posting too?");
                string input = Console.ReadLine();
                
                try
                {
                    userChoice = int.Parse(input);
                    //check if user's choice is in database:
                    bool isThere = false;
                    foreach (int id in blogIDs)
                    {
                        if (id == userChoice) isThere = true;
                    }
                    if (!isThere)
                    {
                        logger.Info("The Blog You Choose Dosen't Exist");
                        //Perhaps the Archives are Incomplete?....
                        loopControl = true;
                    }
                }
                catch(Exception e)
                {
                    loopControl = true;
                    logger.Error(e.Message);
                }
            } while (loopControl);

            //ask for blog title
            string title = "";
            do
            {
                Console.WriteLine("Insert Post Title: ");
                title = Console.ReadLine();
            } while (title.Length <= 1);

            //Ask for blog content
            string content = "";
            do
            {
                Console.WriteLine("Type Content for Post: ");
                content = Console.ReadLine();
            } while (content.Length <= 1);

            //Add post the database
            Post newPost = new Post
            {
                BlogId = userChoice,
                Title = title,
                Content = content
            };
            db.AddPost(newPost);
            logger.Info("Post added - {title}", title);
        }

    }
}
