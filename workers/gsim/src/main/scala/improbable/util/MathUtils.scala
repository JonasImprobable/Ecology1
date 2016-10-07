package improbable.util

import improbable.math.Vector3d

import scala.util.Random

object MathUtils {
  def clamp(value: Float, minVal: Float, maxVal: Float): Float = {
    math.min(math.max(value, minVal), maxVal)
  }

  def getGaussian(mean: Double, standardDeviation: Double): Double = {
    mean + standardDeviation * Random.nextGaussian()
  }

  def getRandomFlatVector(): Vector3d = {
    Vector3d(Random.nextDouble() - 0.5f, 0.0f, Random.nextDouble() - 0.5f).normalised
  }

  def flattenVector(v: Vector3d): Vector3d = {
    v * (Vector3d.unitX + Vector3d.unitZ)
  }

  def getAverageVector(vectors: List[Vector3d]): Vector3d = {
    vectors.fold(Vector3d.zero)(_ + _ / vectors.length)
  }

  def lerp(x0: Float, x1: Float, alpha: Float): Float = {
    (1.0f - alpha) * x0 + alpha * x1
  }

  def generateWhiteNoise(width: Int, height: Int): Array[Array[Float]] = {
    val noise: Array[Array[Float]] = Array.ofDim[Float](width, height)
    Random.setSeed(0)
    for (i <- 0 until width; j <- 0 until height) {
      noise(i)(j) = Random.nextFloat() % 1.0f
    }
    noise
  }

  def generateSmoothNoise(baseNoise: Array[Array[Float]], octave: Int): Array[Array[Float]] = {
    val width: Int = baseNoise.length
    val height: Int = baseNoise(0).length
    val smoothNoise: Array[Array[Float]] = Array.ofDim[Float](width, height)
    val samplePeriod: Int = 1 << octave
    val sampleFrequency: Float = 1.0f / samplePeriod

    for (i <- 0 until width; j <- 0 until height) {
      val sample_i0: Int = (i / samplePeriod) * samplePeriod
      val sample_i1: Int = (sample_i0 + samplePeriod) % width
      val horizontal_blend: Float = (i - sample_i0) * sampleFrequency

      val sample_j0: Int = (j / samplePeriod) * samplePeriod
      val sample_j1: Int = (sample_j0 + samplePeriod) % width
      val vertical_blend: Float = (j - sample_j0) * sampleFrequency

      val top: Float = lerp(baseNoise(sample_i0)(sample_j0), baseNoise(sample_i1)(sample_j0), horizontal_blend)
      val bottom: Float = lerp(baseNoise(sample_i0)(sample_j1), baseNoise(sample_i1)(sample_j1), horizontal_blend);
      smoothNoise(i)(j) = lerp(top, bottom, vertical_blend)
    }
    smoothNoise
  }

  def generatePerlinNoise(baseNoise: Array[Array[Float]], octaveCount: Int, persistence: Float): Array[Array[Float]] = {
    val width: Int = baseNoise.length
    val height: Int = baseNoise(0).length
    val smoothNoise: Array[Array[Array[Float]]] = Array.ofDim[Float](octaveCount, width, height)

    for (i <- 0 until octaveCount) {
      smoothNoise(i) = generateSmoothNoise(baseNoise, i)
    }
    val perlinNoise: Array[Array[Float]] = Array.ofDim[Float](width, height)
    var amplitude: Float = 1.0f
    var totalAmplitude: Float = 0.0f
    for (octave <- octaveCount - 1 to 0 by -1) {
      amplitude *= persistence
      totalAmplitude += amplitude
      for (i <- 0 until width; j <- 0 until height) {
        perlinNoise(i)(j) += smoothNoise(octave)(i)(j) * amplitude
      }
    }
    for (i <- 0 until width; j <- 0 until height) {
      perlinNoise(i)(j) /= totalAmplitude
    }
    perlinNoise
  }

  def generatePerlinNoise(width: Int, height: Int, octaveCount: Int, persistence: Float): Array[Array[Float]] = {
    val baseNoise: Array[Array[Float]] = generateWhiteNoise(width, height)
    generatePerlinNoise(baseNoise, octaveCount, persistence)
  }
}
