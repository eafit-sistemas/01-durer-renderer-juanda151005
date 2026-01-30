using System;
using SkiaSharp;

public static class Program
{
    public static void Main()
    {
        InputData data = InputData.LoadFromJson("input.json");
        Shape2D projected = ProjectShape(data.Model);
        projected.Print (); // The tests check for the correct projected data to be printed
        if (!Environment.GetEnvironmentVariable("CI").Equals("true"))
        {
            Render(projected, data.Parameters, "output.jpg");
        };
    }

    private static void Render(Shape2D shape, RenderParameters parameters, string outputPath)
    {
        List<SKPoint> coordenadas = new();
        
        for (int i=0; i < shape.Points.Length; i++)
        {
            float x=(shape.Points[i][0]-parameters.XMin)/(parameters.XMax-parameters.XMin);
            float y=(shape.Points[i][1]-parameters.YMin)/(parameters.YMax-parameters.YMin);
            x= x*parameters.Resolution;
            y=(1-y)*parameters.Resolution;
            coordenadas.Add(new SKPoint(x,y));
        };

        using SKBitmap bitmap = new(parameters.Resolution, parameters.Resolution);
        using SKCanvas canvas = new(bitmap);
        canvas.Clear(SKColor.Parse("#003366"));
        using SKPaint paint = new()
        {
             Color = SKColors.FloralWhite 
        };

        for (int i=0; i < shape.Lines.Length; i++)
        {
            int a = shape.Lines[i][0];
            int b = shape.Lines[i][1];

            canvas.DrawLine(coordenadas[a], coordenadas[b], paint);
        };
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = File.OpenWrite(outputPath);
        data.SaveTo(stream);

    }

    private static Shape2D ProjectShape(Model3D model)
    {
        float[][] Points=new float[model.VertexTable.Length][];
        for (int i = 0; i < model.VertexTable.Length; i++)
        {
            float X=model.VertexTable[i][0]/model.VertexTable[i][2];
            float Y=model.VertexTable[i][1]/model.VertexTable[i][2];
            Points[i]=[X, Y];
        };
        Shape2D shape = new Shape2D();
        shape.Points=Points;
        shape.Lines=model.EdgeTable;
        return shape;
    }
}

