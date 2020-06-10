using NLog;
using BlogsConsole.Models;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

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
                            if (userChoice > 5 || userChoice <= 0)
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
                            logger.Info("Option 1 Choosen");
                            displayBlogs();
                            break;
                        case 2:
                            logger.Info("Option 2 Choosen");
                            createBlog();
                            break;
                        case 3:
                            logger.Info("Option 3 Choosen");
                            createPost();
                            break;
                        case 4:
                            logger.Info("Option 4 Choosen");
                            displayPosts();
                            break;
                        case 5:
                            logger.Info("Option 5 Choosen");
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
            Console.WriteLine("4: Display Posts");
            Console.WriteLine("5: Exit");
        }

        private static void createBlog()
        {
            // Create and save a new Blog
            string name;
            bool isValidName;
            do
            {
                isValidName = true;
                Console.Write("Enter a name for a new Blog: ");
                name = Console.ReadLine();
                if (name.Equals("") || name.Equals(" "))
                {
                    logger.Error("Cannot have blank Blog name!");
                    isValidName = false;
                }
            } while (!isValidName);
            

            var blog = new Blog { Name = name };

            var db = new BloggingContext();
            db.AddBlog(blog);
            logger.Info("Blog added - {name}", name);
        }

        private static void displayBlogs()
        {
            // Display all Blogs from the database
            var db = new BloggingContext();
            var query = db.Blogs.OrderBy(b => b.BlogId);

            Console.WriteLine(query.ToList().Count+" Blogs Returned");

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
            var queryOne = db.Blogs.OrderBy(b => b.BlogId);
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
                        logger.Error("The Blog You Choose To Post To Dosen't Exist");
                        //Perhaps the Archives are Incomplete?....
                        loopControl = true;
                    }
                }
                catch(FormatException)
                {
                    loopControl = true;
                    logger.Error("Invlaid Blog ID");
                }
                catch(Exception e)
                {
                    loopControl = true;
                    logger.Error(e.Message);
                }
            } while (loopControl);

            //ask for post title
            string title = "";
            bool validTitle;
            do
            {
                validTitle = true;
                Console.WriteLine("Insert Post Title: ");
                title = Console.ReadLine();
                if(title.Equals("")||title.Equals(" "))
                {
                    logger.Error("Post Title Cannot be Blank");
                    validTitle = false;
                }
            } while (!validTitle);

            //Ask for blog content
            Console.WriteLine("Type Content for Post: ");
            string content = Console.ReadLine();
            

            //Add post the database
            Post newPost = new Post
            {
                BlogId = userChoice,
                Title = title,
                Content = content
            };
            db.AddPost(newPost);
            logger.Info("Post added - {title}, to blogID - {userChoice}", title,userChoice);
        }

        private static void displayPosts()
        {
            //Display Posts By Blog

            var db = new BloggingContext();

            //Display Blogs By ID:
            
            var queryOne = db.Blogs.OrderBy(b => b.BlogId);
            ArrayList blogIDs = new ArrayList();
            Console.WriteLine("0: Print All Posts");
            foreach (var item in queryOne)
            {
                Console.WriteLine(item.BlogId + ": Posts from " + item.Name);
                blogIDs.Add(item.BlogId);
            }

            //Force correct choice
            bool loopControl;
            int userChoice = 0;
            do
            {
                loopControl = false;
                Console.WriteLine("Select where to display posts from:");
                string input = Console.ReadLine();

                try
                {
                    userChoice = int.Parse(input);
                    //check if user's choice is in database:
                    bool isThere = false;
                    if (userChoice == 0) isThere = true;
                    else
                    {
                        foreach (int id in blogIDs)
                        {
                            if (id == userChoice) isThere = true;
                        }
                        if (!isThere)
                        {
                            logger.Error("The Blog You Choose To Post To Dosen't Exist");
                            //Perhaps the Archives are Incomplete?....
                            loopControl = true;
                        }
                    }                  
                }
                catch (FormatException)
                {
                    loopControl = true;
                    logger.Error("Invlaid Blog ID");
                }
                catch (Exception e)
                {
                    loopControl = true;
                    logger.Error(e.Message);
                }
            } while (loopControl);


            if(userChoice == 0)
            {
                List<Post> posts = db.Posts.OrderBy( p => p.BlogId).ToList();
                Console.WriteLine(posts.Count() + " posts returned");
                foreach(Post post in posts)
                {
                    List<Blog> blogs = db.Blogs.Where(b => b.BlogId == post.BlogId).ToList();
                    string blogName = "Nothing";
                    foreach (Blog b in blogs) blogName = b.Name;
                    Console.WriteLine("Blog: " + blogName);
                    Console.WriteLine("Title: " + post.Title);
                    Console.WriteLine("Content: " + post.Content);
                }
            }
            else
            {
                List<Post> posts = db.Posts.Where(p => p.BlogId == userChoice).ToList();
                Console.WriteLine(posts.Count() + " posts returned");
                foreach (Post post in posts)
                {
                    List<Blog> blogs = db.Blogs.Where(b => b.BlogId == post.BlogId).ToList();
                    string blogName = "Nothing";
                    foreach (Blog b in blogs) blogName = b.Name;
                    Console.WriteLine("Blog: " + blogName);
                    Console.WriteLine("Title: " + post.Title);
                    Console.WriteLine("Content: " + post.Content);
                }
            }

        }

    }
}
