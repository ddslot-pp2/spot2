#ifndef __PRE_DEFINED_H__
#define __PRE_DEFINED_H__

static const int iphone6_width  = 1920;
static const int iphone6_height = 1080;

static const int offset_x = 2;
static const int offset_y = 50;

static const int image_size_x = (1920 / 2) - offset_x; // 958
static const int image_size_y = 1000 - 20;             // 980

//static const char* req_stage_info_url = "http://127.0.0.1:3000/stage-info/";
static const char* req_stage_info_url = "http://t.05day.com/stage-info/";

// img width  = 958;
// img height = 1080;

struct spot {
  spot(int _x, int _y) : x(_x), y(_y) {}
  int x;
  int y;
};

struct spot_rect {
  spot_rect(int _x, int _y) : x(_x), y(_y) {}
  int x;
  int y;
};

struct stage_info 
{
  std::vector<spot> spots;
  std::vector<spot_rect> spot_rects;
  std::string left_img;
  std::string right_img;
  int current_stage_count;
  int total_stage_count;
  int spot_count;
  float play_time;
};

//std::string tmp_uuid = "51F30A87BBFA128FE915C92391D2F164";
// readPhoneState 퍼미션이 설정되어있을 경우
// ex) 51F30A87 - BBFA - 128F - E915 - C92391D2F164
// readPhoneState 퍼미션이 설정되지 않을 경우
// ex) 8127362567812635
#endif
