using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class SinglePlayForm : Form
    {
        // 15 x 15 바둑판
        private const int rectSize = 33;  // 오목판의 셀 크기
        private const int edgeCount = 15; // 오목판의 선 개수

        private enum Horse
        {
            none = 0,
            BLACK,
            WHITE
        };
        private Horse[,] board = new Horse[edgeCount, edgeCount];  // 바둑판 2차원 배열로 만들기
        private Horse nowPlayer = Horse.BLACK;  // 흑돌 선공

        public SinglePlayForm()
        {
            InitializeComponent();
        }

        // 돌 놓기
        private void boardPicture_MouseDown(object sender, MouseEventArgs e)
        {
            Graphics g = this.boardPicture.CreateGraphics();  // Graphics 객체 생성
            // 마우스 이벤트가 발생했을 때의 위치
            // 몇 번째 셀인지 확인 (0~14)
            int x = e.X / rectSize;  
            int y = e.Y / rectSize;
            if (x < 0 || y < 0 || x >= edgeCount || y >= edgeCount)
            {
                MessageBox.Show("테두리를 벗어날 수 없습니다.");
                return;
            }
            MessageBox.Show(x + ", " + y);
            if (nowPlayer == Horse.BLACK)
            {
                // SolidBrush를 이용해서 좌표, 높이, 너비에 맞는 위치를 칠하기
                SolidBrush brush = new SolidBrush(Color.Black);
                g.FillEllipse(brush, x * rectSize, y * rectSize, rectSize, rectSize);
            }
            else
            {
                SolidBrush brush = new SolidBrush(Color.White);
                g.FillEllipse(brush, x * rectSize, y * rectSize, rectSize, rectSize);
            }
        }


        // 오목판 초기화
        private void boardPicture_Paint(object sender, PaintEventArgs e)
        {
            Graphics gp = e.Graphics;
            Color lineColor = Color.Black;  // 오목판 선의 색깔
            Pen p = new Pen(lineColor, 2);
            // 오목판 테두리 그리기
            gp.DrawLine(p, rectSize / 2, rectSize / 2, rectSize / 2, rectSize * edgeCount - rectSize / 2); // 좌측
            gp.DrawLine(p, rectSize / 2, rectSize / 2, rectSize * edgeCount - rectSize / 2, rectSize / 2); // 상측
            gp.DrawLine(p, rectSize / 2, rectSize * edgeCount - rectSize / 2, rectSize * edgeCount - rectSize / 2, rectSize * edgeCount - rectSize / 2);  // 하측
            gp.DrawLine(p, rectSize * edgeCount - rectSize / 2, rectSize / 2, rectSize * edgeCount - rectSize / 2, rectSize * edgeCount - rectSize / 2);  // 우측
            p = new Pen(lineColor, 1);
            // 대각선 방향으로 이동하면서 십자가 모양의 선 그리기
            for (int i= rectSize + rectSize /2; i <rectSize * edgeCount - rectSize / 2; i+= rectSize)
            {
                gp.DrawLine(p, rectSize / 2, i, rectSize * edgeCount - rectSize / 2, i);
                gp.DrawLine(p, i, rectSize / 2, i, rectSize * edgeCount - rectSize / 2);
            }




        }
    }
}
