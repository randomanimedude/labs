#include <opencv.hpp>

#include <core.hpp>

#include <highgui.hpp>

int main()
{
    cv::Mat image = cv::imread("1.png");

    cv::namedWindow("Image:", 1);

    cv::imshow("Image:", image);

    cv::waitKey();

    cv::destroyWindow("Image:");

    return 0;
}