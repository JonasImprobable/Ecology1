import improbable.math.Vector3d
import scala.util.Random

def getAverageVector(vectors: List[Vector3d]): Vector3d = {
  vectors.fold(Vector3d.zero)(_ + _ / vectors.length)
}

var ll = List[Vector3d](Vector3d(2, 3, 4), Vector3d(1, 1, -1), Vector3d(4, 2, 10))
getAverageVector(ll)

val foo = 26f
val foo2 = foo % 24

/*
val list = List[Vector3d](Vector3d(1,1,1), Vector3d(1,1,1), Vector3d(1,1,1))
list.fold(Vector3d.zero)(_+_)
val A = Map[Int, Int](
  1 -> 100,
  2 -> 200,
  3 -> 300,
  4 -> 400
)
A.filter((k: Int,v: String) => {k % 2 == 0})
A.filter(t => t._2 > 250)
A.map((t => (t._1,t._2*2))
var v = List(('a', 2), ('b', 1))
v.sortBy(t => t._2)
val l = List[Int](1)
l.head
l.tail
def foo(i: Int): Int = {
  if (i == 2) return -1
  i + 2
}
val coco: List[Int] = List(1, 2, 3)
(coco :+ 4).filter(elem => elem != 2)
val arr = List.range(0,100 )
val buff = scala.collection.mutable.ListBuffer.empty[Double]
arr.foreach {i => buff += math.acos(i/100.0)/math.Pi }//math.acos(i)}
buff
val v1 = Vector3d(1, 0, 0)
math.atan2(0, -1)/math.Pi
val angle =  0.1
(math.min((angle + 1.0)/2.0, 0.9999) * 8.0 ).asInstanceOf[Int]
var li = scala.collection.mutable.ListBuffer.empty[Int]
for (i <- 10 until 0 by -1) li += i
val ar = Array.ofDim[Float](5,4)
ar(0)(0) += 1
//ar.deep.mkString("\n")
//scala.runtime.ScalaRunTime.stringOf(ar)
ar.foreach{case a => a foreach {b => print(b.toString + ", ")}; print('\n')}

val arr = new Array[Double](40)
for (i <- 0 until 40) {
  arr(i) = Random.nextGaussian()
}
arr.foreach{e => print(e + ", ")}
*/