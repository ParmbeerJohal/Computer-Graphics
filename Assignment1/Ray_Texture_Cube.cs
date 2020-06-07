/* Parm Johal
 * V00787710
 * CSC 305 Assignment 1
 * Ray-Triangle intersection
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class Texture_Cube : MonoBehaviour
{

    //initialize variables
    public Texture2D texture_on_cube;
    Texture2D RayTracingResult;

    void Start()
    {
        Camera this_camera = gameObject.GetComponent<Camera>();
        Debug.Assert(this_camera);

        int pixel_width = this_camera.pixelWidth;
        int pixel_height = this_camera.pixelHeight;

        RayTracingResult = new Texture2D(pixel_width, pixel_height);

        #region Generate a black and white checker pattern

        for (int i = 0; i < pixel_width; ++i)
            for (int j = 0; j < pixel_height; ++j)
            {
                int iblock = i / 50;
                int jblock = j / 50;
                RayTracingResult.SetPixel(i, j,
                    (iblock + jblock) % 2 == 0 ? Color.black : Color.white);
            }
        #endregion


        //Please put your own ray tracing code here
        Vector3 RayOrigin = Vector3.zero;
        Vector3 VPCenter = Vector3.forward;
        Vector3 RayDirection = VPCenter;
        float Viewportwidth = 3;
        float Viewportheight = Viewportwidth / pixel_width * pixel_height;
        //half of view port
        float VPWidthHalf = Viewportwidth / 2;
        float VPHeightHalf = Viewportheight / 2;
        //half of pixels
        float PixelWidthHalf = pixel_width / 2;
        float PixelHeightHalf = pixel_height / 2;
        //colours
        Color BackgroundColor = Color.grey;
        Color pixelColor;

        //triangle initializations
        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(new Vector3(-4, -2.8f, 10)); //0
        vertices.Add(new Vector3(-4, 2.8f, 10));  //1
        vertices.Add(new Vector3(0, -2.8f, 9));   //2
        vertices.Add(new Vector3(0, 2.8f, 9));    //3
        vertices.Add(new Vector3(4, -2.8f, 10));  //4
        vertices.Add(new Vector3(4, 2.8f, 10));   //5

        // Triangle indices
        List<int> indices = new List<int>();
        //triangle 1
        indices.Add(0);
        indices.Add(1);
        indices.Add(2);
        //triangle 2
        indices.Add(2);
        indices.Add(1);
        indices.Add(3);
        //triangle 3
        indices.Add(2);
        indices.Add(3);
        indices.Add(5);
        //triangle 4
        indices.Add(2);
        indices.Add(5);
        indices.Add(4);

        List<Vector2> UVList = new List<Vector2>();
        UVList.Add(new Vector2(0, 0)); //0
        UVList.Add(new Vector2(0, 1)); //1
        UVList.Add(new Vector2(1, 0)); //2
        UVList.Add(new Vector2(1, 1)); //3
        UVList.Add(new Vector2(0, 0)); //4
        UVList.Add(new Vector2(0, 1)); //5


        for (int x = 0; x < pixel_width; ++x)
        {
            for (int y = 0; y < pixel_height; ++y)
            {
                RayDirection.x = (x - PixelWidthHalf) / PixelWidthHalf * VPWidthHalf;
                RayDirection.y = (y - PixelHeightHalf) / PixelHeightHalf * VPHeightHalf;

                RayTracingResult.SetPixel(x, y, Color.grey);
                pixelColor = BackgroundColor;
                RayDirection.Normalize();

                //ray direction
                float ray_x = RayDirection.x;
                float ray_y = RayDirection.y;
                float ray_z = RayDirection.z;

                //Intersection between the points of the triangles

                //vA - vB
                float a = vertices[0].x - vertices[1].x;
                float b = vertices[0].y - vertices[1].y;
                float c = vertices[0].z - vertices[1].z;
                //vA - vC
                float d = vertices[0].x - vertices[2].x;
                float e = vertices[0].y - vertices[2].y;
                float f = vertices[0].z - vertices[2].z;
                //origin of the ray
                float g = vertices[0].x - RayOrigin.x;
                float h = vertices[0].y - RayOrigin.y;
                float i = vertices[0].z - RayOrigin.z;

                Vector3 vA_vB = new Vector3(a, b, c);
                Vector3 vA_vC = new Vector3(d, e, f);
                Vector3 normal = Vector3.Cross(vA_vB, vA_vC);

                float t = Vector3.Dot(normal, vertices[0] - RayOrigin) / Vector3.Dot(normal, RayDirection);

                //Cramer's rule
                float ak_jb = a * h - g * b;
                float jc_al = g * c - a * i;
                float bl_kc = b * i - h * c;
                float ei_hf = e * ray_z - ray_y * f;
                float gf_di = ray_x * f - d * ray_z;
                float dh_eg = d * ray_y - e * ray_x;


                float M = a * ei_hf + b * gf_di + c * dh_eg;
                float beta = (g * (ei_hf) + h * (gf_di) + i * (dh_eg)) / M;
                float gamma = (ray_z * (ak_jb) + ray_y * (jc_al) + ray_x * (bl_kc)) / M;

                float alpha = 1 - beta - gamma;
                Vector3 barycentric_coordinate = new Vector3(alpha, beta, gamma);

                //this changes the points to barycentric coordinates
                if (t > 0) //if intersect, apply the alpha, beta, gamma values to r,g,b
                {
                    if (gamma > 0 && beta > 0 && (beta + gamma) < 1)
                    {
                        Vector2 value = (alpha * UVList[0]) + (beta * UVList[1]) + (gamma * UVList[2]);

                        pixelColor = texture_on_cube.GetPixelBilinear(value.x, value.y); //get pixel color at pixel location of image
                        RayTracingResult.SetPixel(x, y, pixelColor);
                    }
                }
            }
        }
        for (int x = 0; x < pixel_width; ++x)
        {
            for (int y = 0; y < pixel_height; ++y)
            {
                RayDirection.x = (x - PixelWidthHalf) / PixelWidthHalf * VPWidthHalf; //current pixel - half of pixel width / half of pixel width * view port width
                RayDirection.y = (y - PixelHeightHalf) / PixelHeightHalf * VPHeightHalf;

                //RayTracingResult.SetPixel(ii, jj, Color.grey);
                pixelColor = BackgroundColor; //set pixel colour the same as background. When pixels intersect the object, change the colour
                RayDirection.Normalize(); //normalize ray direction

                //ray direction
                float ray_x = RayDirection.x;
                float ray_y = RayDirection.y;
                float ray_z = RayDirection.z;

                //Intersection between the points of the triangles

                //vA - vB
                float a = vertices[2].x - vertices[1].x;
                float b = vertices[2].y - vertices[1].y;
                float c = vertices[2].z - vertices[1].z;
                //vA - vC
                float d = vertices[2].x - vertices[3].x;
                float e = vertices[2].y - vertices[3].y;
                float f = vertices[2].z - vertices[3].z;
                //origin of the ray
                float g = vertices[2].x - RayOrigin.x;
                float h = vertices[2].y - RayOrigin.y;
                float i = vertices[2].z - RayOrigin.z;

                Vector3 vA_vB = new Vector3(a, b, c);
                Vector3 vA_vC = new Vector3(d, e, f);
                Vector3 normal = Vector3.Cross(vA_vB, vA_vC);

                float t = Vector3.Dot(normal, vertices[2] - RayOrigin) / Vector3.Dot(normal, RayDirection);

                //cramer's rule
                float ak_jb = a * h - g * b;
                float jc_al = g * c - a * i;
                float bl_kc = b * i - h * c;
                float ei_hf = e * ray_z - ray_y * f;
                float gf_di = ray_x * f - d * ray_z;
                float dh_eg = d * ray_y - e * ray_x;


                float M = a * ei_hf + b * gf_di + c * dh_eg;
                float beta = (g * (ei_hf) + h * (gf_di) + i * (dh_eg)) / M;
                float gamma = (ray_z * (ak_jb) + ray_y * (jc_al) + ray_x * (bl_kc)) / M;
                float alpha = 1 - beta - gamma;
                Vector3 barycentric_coordinate = new Vector3(alpha, beta, gamma);

                //this changes the points to barycentric coordinates
                if (t > 0) //if intersect, apply the alpha, beta, gamma values to r,g,b
                {
                    if (gamma > 0 && beta > 0 && (beta + gamma) < 1)
                    {
                        Vector2 value = (alpha * UVList[2]) + (beta * UVList[1]) + (gamma * UVList[3]);

                        pixelColor = texture_on_cube.GetPixelBilinear(value.x, value.y);
                        RayTracingResult.SetPixel(x, y, pixelColor);
                    }
                }
            }
        }

        for (int x = 0; x < pixel_width; ++x)
        {
            for (int y = 0; y < pixel_height; ++y)
            {
                RayDirection.x = (x - PixelWidthHalf) / PixelWidthHalf * VPWidthHalf; //current pixel - half of pixel width / half of pixel width * view port width
                RayDirection.y = (y - PixelHeightHalf) / PixelHeightHalf * VPHeightHalf;

                pixelColor = BackgroundColor;
                RayDirection.Normalize();

                //ray direction
                float g = RayDirection.x;
                float h = RayDirection.y;
                float i = RayDirection.z;

                //Intersection between the points of the triangles

                //vA - vB
                float a = vertices[2].x - vertices[3].x;
                float b = vertices[2].y - vertices[3].y;
                float c = vertices[2].z - vertices[3].z;
                //vA - vC
                float d = vertices[2].x - vertices[5].x;
                float e = vertices[2].y - vertices[5].y;
                float f = vertices[2].z - vertices[5].z;
                //origin of the ray
                float j = vertices[2].x - RayOrigin.x;
                float k = vertices[2].y - RayOrigin.y;
                float l = vertices[2].z - RayOrigin.z;

                Vector3 vA_vB = new Vector3(a, b, c);
                Vector3 vA_vC = new Vector3(d, e, f);
                Vector3 normal = Vector3.Cross(vA_vB, vA_vC);

                float t = Vector3.Dot(normal, vertices[2] - RayOrigin) / Vector3.Dot(normal, RayDirection);

                //cramer's rule
                float ak_jb = a * k - j * b;
                float jc_al = j * c - a * l;
                float bl_kc = b * l - k * c;
                float ei_hf = e * i - h * f;
                float gf_di = g * f - d * i;
                float dh_eg = d * h - e * g;


                float M = a * ei_hf + b * gf_di + c * dh_eg;
                float beta = (j * (ei_hf) + k * (gf_di) + l * (dh_eg)) / M;
                float gamma = (i * (ak_jb) + h * (jc_al) + g * (bl_kc)) / M;

                float alpha = 1 - beta - gamma;
                Vector3 barycentric_coordinate = new Vector3(alpha, beta, gamma);

                //this changes the points to barycentric coordinates
                if (t > 0) //if intersect, apply the alpha, beta, gamma values to r,g,b
                {
                    if (gamma > 0 && beta > 0 && (beta + gamma) < 1)
                    {
                        Vector2 value = (alpha * UVList[2]) + (beta * UVList[3]) + (gamma * UVList[5]);

                        pixelColor = texture_on_cube.GetPixelBilinear(value.x, value.y); //get pixel color at pixel location of image
                        RayTracingResult.SetPixel(x, y, pixelColor);
                    }
                }
            }
        }

        for (int x = 0; x < pixel_width; ++x)
        {
            for (int y = 0; y < pixel_height; ++y)
            {
                RayDirection.x = (x - PixelWidthHalf) / PixelWidthHalf * VPWidthHalf; //current pixel - half of pixel width / half of pixel width * view port width
                RayDirection.y = (y - PixelHeightHalf) / PixelHeightHalf * VPHeightHalf;

                pixelColor = BackgroundColor;
                RayDirection.Normalize();

                //Ray direction
                float g = RayDirection.x;
                float h = RayDirection.y;
                float i = RayDirection.z;

                //Intersection betweeen the points of the triangle

                //vA - vB
                float a = vertices[2].x - vertices[5].x;
                float b = vertices[2].y - vertices[5].y;
                float c = vertices[2].z - vertices[5].z;
                //vA - vC
                float d = vertices[2].x - vertices[4].x;
                float e = vertices[2].y - vertices[4].y;
                float f = vertices[2].z - vertices[4].z;
                //origin of the ray
                float j = vertices[2].x - RayOrigin.x;
                float k = vertices[2].y - RayOrigin.y;
                float l = vertices[2].z - RayOrigin.z;

                Vector3 vA_vB = new Vector3(a, b, c);
                Vector3 vA_vC = new Vector3(d, e, f);
                Vector3 normal = Vector3.Cross(vA_vB, vA_vC);
                float t = Vector3.Dot(normal, vertices[2] - RayOrigin) / Vector3.Dot(normal, RayDirection);

                //cramer's rule
                float ak_jb = a * k - j * b;
                float jc_al = j * c - a * l;
                float bl_kc = b * l - k * c;
                float ei_hf = e * i - h * f;
                float gf_di = g * f - d * i;
                float dh_eg = d * h - e * g;

                float M = a * ei_hf + b * gf_di + c * dh_eg;
                float beta = (j * (ei_hf) + k * (gf_di) + l * (dh_eg)) / M;
                float gamma = (i * (ak_jb) + h * (jc_al) + g * (bl_kc)) / M;

                float alpha = 1 - beta - gamma;
                Vector3 barycentric_coordinate = new Vector3(alpha, beta, gamma);

                //this changes the points to barycentric coordinates
                if (t > 0) //if intersect, apply the alpha, beta, gamma values to r,g,b
                {
                    if (gamma > 0 && beta > 0 && (beta + gamma) < 1)
                    {
                        Vector2 value = (alpha * UVList[2]) + (beta * UVList[5]) + (gamma * UVList[4]);

                        pixelColor = texture_on_cube.GetPixelBilinear(value.x, value.y);
                        RayTracingResult.SetPixel(x, y, pixelColor);
                    }
                }
            }
        }
        RayTracingResult.Apply();


    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //Show the generated ray tracing image on screen
        Graphics.Blit(RayTracingResult, destination);
    }
}