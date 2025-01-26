using TaskList;

if (args.Length > 0)
{
    CliProgram.Main(args);
}
else
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddSingleton<ITaskListService, TaskListService>();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddControllers(); // Register controllers

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers(); // Map controllers

    var taskListService = app.Services.GetRequiredService<ITaskListService>();

    System.Threading.Tasks.Task.Run(() =>
    {
        // Start CLI
        new TaskList.TaskList(new RealConsole(), taskListService).Run();
    });

    app.Run();
}